using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace JsonLocalization
{
    sealed class LocalizerOptionsFactory<TOptions> : IOptionsFactory<TOptions>
         where TOptions : class, new()
    {
        private readonly IConfigureOptions<TOptions>[] _setups;
        private readonly IPostConfigureOptions<TOptions>[] _postConfigures;
        private readonly IValidateOptions<TOptions>[] _validations;


        public LocalizerOptionsFactory(IEnumerable<IConfigureOptions<TOptions>> setups, IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, IEnumerable<IValidateOptions<TOptions>> validations)
        {
            _setups = setups as IConfigureOptions<TOptions>[] ?? setups.ToArray();
            _postConfigures = postConfigures as IPostConfigureOptions<TOptions>[] ?? postConfigures.ToArray();
            _validations = validations as IValidateOptions<TOptions>[] ?? validations.ToArray();
        }

      
        public TOptions Create(string name)
        {
            var defaultOptions = this.CreateOptions(Options.DefaultName, default);
            if (string.IsNullOrEmpty(name))
            {
                return defaultOptions;
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

            var options = defaultOptions;
            while (stack.TryPop(out var next))
            {
                options = this.CreateOptions(next, options);
            }

            return options;
        }

     
        private TOptions CreateOptions(string name, TOptions? options)
        {
            if (options == null)
            {
                options = new TOptions();
            }

            foreach (var setup in _setups)
            {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
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
                    throw new OptionsValidationException(name, typeof(TOptions), failures);
                }
            }

            return options;
        }
    }
}