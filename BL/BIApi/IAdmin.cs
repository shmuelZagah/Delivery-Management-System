using BO;

namespace BIApi;

public interface IAdmin
{
    /// <summary>
    /// Resets the database.
    /// Reverts all configuration parameters to their initial values and clears all entity data lists.
    /// </summary>
    void ResetDB();


    /// <summary>
    /// Initializes the database.
    /// First performs a full reset, then populates the entity lists with initial data according to requirements.
    /// </summary>
    void InitializeDB();


    /// <summary>
    /// Retrieves the current system clock value.
    /// </summary>
    /// <returns>The current system time as a DateTime object.</returns>
    DateTime GetClock();


    void ForwardClock(TimeUnit time);

    /// <summary>
    /// Retrieves configuration variables relevant to the presentation layer.
    /// Excludes internal variables such as running counters.
    /// </summary>
    /// <returns>A BO.Config object containing the configuration values.</returns>
    BO.Config? GetConfig();


    /// <summary>
    /// Updates configuration variables based on the provided object.
    /// Updates only the variables relevant to the presentation layer that have changed since the last update.
    /// </summary>
    /// <param name="config">The BO.Config object containing the new values.</param>
    public void SetConfig(BO.Config config);
}
