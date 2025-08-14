using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace JsonLocalization
{
    sealed class LocaleOptionsFactory<TLocale> : IOptionsFactory<TLocale>
         where TLocale : class, new()
    {
        private readonly IConfigureOptions<TLocale>[] _setups;
        private readonly IPostConfigureOptions<TLocale>[] _postConfigures;
        private readonly IValidateOptions<TLocale>[] _validations;


        public LocaleOptionsFactory(IEnumerable<IConfigureOptions<TLocale>> setups, IEnumerable<IPostConfigureOptions<TLocale>> postConfigures, IEnumerable<IValidateOptions<TLocale>> validations)
        {
            _setups = setups as IConfigureOptions<TLocale>[] ?? setups.ToArray();
            _postConfigures = postConfigures as IPostConfigureOptions<TLocale>[] ?? postConfigures.ToArray();
            _validations = validations as IValidateOptions<TLocale>[] ?? validations.ToArray();
        }

        /// <summary>
        /// 创建TLocale
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TLocale Create(string name)
        {
            var defaultLocale = this.CreateCore(Options.DefaultName, default);
            if (string.IsNullOrEmpty(name))
            {
                return defaultLocale;
            }

            int index;
            var culture = name;
            var stack = new Stack<string>();
            stack.Push(culture);

            while ((index = culture.LastIndexOf('-')) >= 0)
            {
                culture = culture[..index];
                stack.Push(culture);
            }

            var locale = defaultLocale;
            while (stack.TryPop(out var next))
            {
                locale = this.CreateCore(next, locale);
            }

            return locale;
        }

        /// <summary>
        /// 创建TLocale
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options">传入的实例</param>
        /// <returns></returns>
        private TLocale CreateCore(string name, TLocale? options)
        {
            if (options == null)
            {
                options = new TLocale();
            }

            foreach (var setup in _setups)
            {
                if (setup is IConfigureNamedOptions<TLocale> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else if (name == Options.DefaultName)
                {
                    setup.Configure(options);
                }
            }

            foreach (var post in _postConfigures)
            {
                post.PostConfigure(name, options);
            }

            if (_validations != null)
            {
                var failures = new List<string>();
                foreach (var validate in _validations)
                {
                    var result = validate.Validate(name, options);
                    if (result != null && result.Failed)
                    {
                        failures.AddRange(result.Failures);
                    }
                }
                if (failures.Count > 0)
                {
                    throw new OptionsValidationException(name, typeof(TLocale), failures);
                }
            }

            return options;
        }
    }
}