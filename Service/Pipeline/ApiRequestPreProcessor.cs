using FluentValidation;
using MediatR.Pipeline;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Pipeline
{
    public class ApiRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IEnumerable<IValidator> _validators;

        public ApiRequestPreProcessor(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return Task.CompletedTask;
            }

            var context = new ValidationContext(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var ex = new ValidationException(failures);
                var payload = JsonConvert.SerializeObject(request);
                var typeOf = request.GetType().Name;
                throw ex;
            }
            return Task.CompletedTask;
        }
    }
}
