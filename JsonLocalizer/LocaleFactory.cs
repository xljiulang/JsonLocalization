using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace JsonLocalizer
{
  
    sealed class LocaleFactory<TLocale> : IOptionsFactory<TLocale>
         where TLocale : class, new()
    {
        private readonly IConfigureOptions<TLocale>[] _setups;
        private readonly IPostConfigureOptions<TLocale>[] _postConfigures;
        private readonly IValidateOptions<TLocale>[] _validations;

        
        public LocaleFactory(IEnumerable<IConfigureOptions<TLocale>> setups, IEnumerable<IPostConfigureOptions<TLocale>> postConfigures, IEnumerable<IValidateOptions<TLocale>> validations)
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
            var defaultOptions = this.Create(Options.DefaultName, default);
            if (string.IsNullOrEmpty(name))
            {
                return defaultOptions;
            }

            var index = name.IndexOf('-');
            if (index < 0)
            {
                return this.Create(name, defaultOptions);
            }

            var langName = name[..index];
            var langOptions = this.Create(langName, defaultOptions);
            return this.Create(name, langOptions);
        }

        /// <summary>
        /// 创建TLocale
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options">传入的实例</param>
        /// <returns></returns>
        private TLocale Create(string name, TLocale? options)
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