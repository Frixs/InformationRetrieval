﻿using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The unit of work class serves one purpose: to make sure that when you use multiple repositories, they share a single database context. 
    /// That way, when a unit of work is complete you can call the <see cref="Commit"/> method on that instance of the context and be assured that all related changes will be coordinated.
    /// </summary>
    internal sealed class UnitOfWork : IUnitOfWork
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members

        /// <summary>
        /// The database context (DI inject)
        /// </summary>
        private ApplicationDbContext _dbContext;

        /// <summary>
        /// Private reference of property <see cref="ApplicationState"/>
        /// </summary>
        private IApplicationStateRepository _applicationState;

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public IApplicationStateRepository ApplicationState
        {
            get
            {
                return _applicationState ??
                    (_applicationState = new ApplicationStateRepository(_dbContext));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnitOfWork(ApplicationDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public void Commit()
        {
            int n = _dbContext.SaveChanges();
            _logger.LogDebugSource($"Total of {n} database changes saved!");
        }

        /// <inheritdoc/>
        public async Task EnsureDatabaseCreatedAsync()
        {
            // Make sure the database exists and is created
            await _dbContext.Database.EnsureCreatedAsync();
        }

        #endregion
    }
}