
namespace Ashutosh.Common.Logger
{
    ///<summary>
    ///Specifies how the event affects the system functionality.
    ///Would give an idea of the consequence of a particular event.
    ///</summary>
    public enum Severity
    {
        /// <summary>
        /// Default value no severity set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates lower priority Debug log.
        ///  </summary>
        Verbose = 1,
        
        /// <summary>
        /// Indicates an information log.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Indicates a warning log.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Indicates an error log
        /// </summary>
        Error = 4,

        /// <summary>
        /// Indicates a fatal error in the system
        /// </summary>
        Fatal = 5
        
    }
}
