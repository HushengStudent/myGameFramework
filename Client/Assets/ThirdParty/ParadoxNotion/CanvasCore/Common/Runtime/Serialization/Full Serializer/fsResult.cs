using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// The result of some sort of operation. A result is either successful or not, but if it
    /// is successful then there may be a set of warnings/messages associated with it. These
    /// warnings describe the performed error recovery operations.
    public struct fsResult
    {

        // We cache the empty string array so we can unify some collections processing code.
        private static readonly string[] EmptyStringArray = { };

        /// Is this result successful?
        private bool _success;

        /// The warning or error messages associated with the result if any.
        private List<string> _messages;

        /// A successful result.
        public static fsResult Success = new fsResult { _success = true };

        /// Create a result that is successful but contains the given warning message.
        public static fsResult Warn(string warning) {
            return new fsResult
            {
                _success = true,
                _messages = new List<string> { warning }
            };
        }

        /// Create a result that failed.
        public static fsResult Fail(string warning) {
            return new fsResult
            {
                _success = false,
                _messages = new List<string> { warning }
            };
        }

        /// Adds a new message to this result.
        public void AddMessage(string message) {
            if ( _messages == null ) { _messages = new List<string>(); }
            _messages.Add(message);
        }

        /// Adds only the messages from the other result into this result, ignoring
        /// the success/failure status of the other result.
        public void AddMessages(fsResult result) {
            if ( result._messages == null ) { return; }
            if ( _messages == null ) { _messages = new List<string>(); }
            _messages.AddRange(result._messages);
        }

        /// Merges the other result into this one. If the other result failed, then
        /// this one too will have failed.
        fsResult Merge(fsResult other) {
            // Copy success/messages over
            _success = _success && other._success;
            if ( other._messages != null ) {
                if ( _messages == null ) _messages = new List<string>(other._messages);
                else _messages.AddRange(other._messages);
            }
            return this;
        }

        /// Only use this as +=!
        public static fsResult operator +(fsResult a, fsResult b) {
            return a.Merge(b);
        }

        /// Did this result fail? If so, you can see the reasons why in `RawMessages`.
        public bool Failed { get { return _success == false; } }

        /// Was the result a success? Note that even successful operations may have
        /// warning messages (`RawMessages`) associated with them.
        public bool Succeeded { get { return _success; } }

        /// Does this result have any warnings? This says nothing about if it failed
        /// or succeeded, just if it has warning messages associated with it.
        public bool HasWarnings { get { return _messages != null && _messages.Any(); } }

        /// A simply utility method that will assert that this result is successful. If it
        /// is not, then an exception is thrown.
        public fsResult AssertSuccess() {
            if ( Failed ) { throw AsException; }
            return this;
        }

        /// A simple utility method that will assert that this result is successful and that
        /// there are no warning messages. This throws an exception if either of those
        /// asserts are false.
        public fsResult AssertSuccessWithoutWarnings() {
            if ( Failed || RawMessages.Any() ) { throw AsException; }
            return this;
        }

        /// Utility method to convert the result to an exception. This method is only defined
        /// is `Failed` returns true.
        public Exception AsException {
            get
            {
                if ( !Failed && !RawMessages.Any() ) throw new Exception("Only a failed result can be converted to an exception");
                return new Exception(FormattedMessages);
            }
        }

        public IEnumerable<string> RawMessages {
            get
            {
                if ( _messages != null ) {
                    return _messages;
                }
                return EmptyStringArray;
            }
        }

        public string FormattedMessages {
            get
            {
                return string.Join(",\n", RawMessages.ToArray());
            }
        }
    }
}