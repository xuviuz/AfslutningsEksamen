using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSClient
{
    /// <summary>
    /// Wrapper for return messages
    /// </summary>
    class ReturnMessageWrapper
    {
        /// <summary>
        /// Gets or sets ResponseToClientid
        /// </summary>
        public string ResponseToClientid { get; set; }

        /// <summary>
        /// Gets or sets ResponseToMessageid
        /// </summary>
        public string ResponseToMessageid { get; set; }

        /// <summary>
        /// Gets or sets data (json serialized object, which have been converted to utf8 byte array)
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        public LocalizedString[] Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnMessageWrapper"/> class.
        /// </summary>
        /// <param name="responseToClientid">Which client is to receive the message</param>
        /// <param name="responseToMessageid">What is the clients message id</param>
        /// <param name="data">the return object as json (utf8 byte array)</param>
        public ReturnMessageWrapper(string responseToClientid, string responseToMessageid, byte[] data)
        {
            this.ResponseToClientid = responseToClientid;
            this.ResponseToMessageid = responseToMessageid;
            this.Data = data;
        }
    }
}
