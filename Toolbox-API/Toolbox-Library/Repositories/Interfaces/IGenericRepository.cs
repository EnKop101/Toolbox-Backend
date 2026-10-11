using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Toolbox_Library.Repositories.Interfaces
{
    /// <summary>
    /// A Generic Repository made for interacting and handling the database.
    /// </summary>
    /// <remarks>
    /// Explaination of a Generic function with Expression parameters.
    /// params : allows the user to pass comma seperated values that it then makes into an array instead of the user having to make the array.
    /// Func : Just a delegate that takes and input (T) and returns and output (object).
    /// Expression : An EF Core thing that tells the code not to compile the Func as a normal delegate which then allows EF to actually read the function.
    /// </remarks>
    /// <typeparam name="T">A Generic object that is set during instantiation of the repository inside the Service</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Used to get a Entity Class from the database.
        /// </summary>
        /// <param name="predicate">Used as a "Where" condition.</param>
        /// <param name="includes">An optional parameter for including a Navigation Property of a entity.</param>
        /// <returns>Can return the T Entity class or null if nothing matching the predicate was found.</returns>
        public Task<T?> Get(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Used to get a list of an Entity Class from the database
        /// </summary>
        /// <param name="predicate">Optional predicate for filtering data with a Where clause</param>
        /// <param name="includes">Optional parameter for including a Navigation Property of a entity.</param>
        /// <returns></returns>
        public Task<List<T>> GetAll(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Used to just check if the Entity exists in the database based on some conditions(predicate)
        /// </summary>
        /// <param name="predicate">Used as a "Where" condition.</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DoesValueExist(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds the Entity to the database and saves the changes.
        /// </summary>
        /// <param name="entity">The Entity Class that needs to be added</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> Add(T entity);

        /// <summary>
        /// Adds a list of Entities all at once to the database and saves the changes.
        /// </summary>
        /// <param name="entities">A list of Entities to be added.</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> AddRange(List<T> entities);

        /// <summary>
        /// Updates / Changes an existing Entity in the database with new values.
        /// </summary>
        /// <param name="entity">The updated Entity version with the new values</param>
        /// <returns>The updated Entity with the new values.</returns>
        public Task<bool> Update(T entity);

        /// <summary>
        /// Deletes a Entity based of a condition(predicate) from the database and saves the changes.
        /// </summary>
        /// <param name="predicate">Used as a "Where" condition.</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Removes a list of a Entities from the database and saves the changes.
        /// </summary>
        /// <param name="entities">The list of entities that needs to be deleted.</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> RemoveRange(List<T> entities);

        /// <summary>
        /// Returns a count of the amount of entities based of a condition(predicate)
        /// </summary>
        /// <param name="predicate">Used as a "Where" condition.</param>
        /// <returns>The Count.</returns>
        public Task<int> Count(Expression<Func<T, bool>>? predicate = null);
    }
}
