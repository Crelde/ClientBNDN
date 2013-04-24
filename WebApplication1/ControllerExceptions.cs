using System;

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

    /// <summary>
    /// Thrown in situations where an attempt to receive an
    /// object from the service failed.
    /// </summary>
    public class ObjectNotFoundException : Exception { }

    /// <summary>
    /// Thrown in situations where one attempts to add an
    /// object that has the some key values as one that
    /// already exists.
    /// E.g: Creating a User with an email which is already
    /// in use.
    /// </summary>
    public class KeyOccupiedException : Exception { }

    /// <summary>
    /// Thrown in situations where one attempts to update
    /// a non-existant object.
    /// E.g: Calling UpdateUser with a User object
    /// whose Email property does not match an existing user.
    /// </summary>
    public class OriginalNotFoundException : Exception { }
}