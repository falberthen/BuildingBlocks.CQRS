﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS.Core;
using FluentValidation.Results;

namespace BuildingBlocks.CQRS.QueryHandling
{
    /// <summary>
    /// Abstract class meant to be inherited by Query Handlers
    /// </summary>
    /// <typeparam name="TQuery">Object of TQuery</typeparam>
    /// <typeparam name="TResult">Dynamic object</typeparam>
    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, QueryHandlerResult<TResult>>
        where TQuery : IQuery<QueryHandlerResult<TResult>>
    {
        protected ValidationResult ValidationResult = new ValidationResult();

        /// <summary>
        /// To override
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<TResult> ExecuteQuery(TQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// MediatR Handle implementation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<QueryHandlerResult<TResult>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            if ((dynamic)query == null)
                throw new ArgumentNullException(nameof(query));

            QueryHandlerResult<TResult> result = new QueryHandlerResult<TResult>(query);

            try
            {
                if (result.ValidationResult.IsValid)
                    result.Result = await ExecuteQuery(query, cancellationToken);
            }
            catch (Exception e)
            {
                result.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, e.Message));
            }

            return result;
        }
    }
}