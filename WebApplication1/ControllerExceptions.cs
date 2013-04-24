using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    /// <summary>
    /// Thrown in situations where the invoker is 
    /// required to be logged in, but is not.
    /// </summary>
    public class NotLoggedInException : Exception { }

    /// <summary>
    /// Thrown in situations where the invoker is
    /// required to be logged out, but is not.
    /// </summary>
    public class NotLoggedOutException : Exception { }

    /// <summary>
    /// Thrown in situations where an action requires
    /// the invoker to have some specifics rights
    /// that it does not fulfill.
    /// </summary>
    public class InsufficientRightsException : Exception { }

    public class NoSuchUserException : Exception { }

    /// <summary>
    /// Thrown in situations where a passwords is required
    /// and the one given does not match.
    /// </summary>
    public class IncorrectPasswordException : Exception { }

    /// <summary>
    /// Thrown in situations where given data does not meet
    /// the required standards.
    /// E.g: A given FileInfo does not have all its required
    /// properties initialized, and the Name property is
    /// not a string of at least 3 characters.
    /// </summary>
    public class InadequateObjectException : Exception { }
}