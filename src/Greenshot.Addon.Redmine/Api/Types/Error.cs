using System;
using System.Collections.Generic;
using System.Text;

namespace Greenshot.Addon.Redmine.Api.Types
{
    /// <summary>
    /// HTTP status codes 422 (Unprocessable Entity) response with the error messages in its body
    /// <see href="http://www.redmine.org/projects/redmine/wiki/Rest_api#Validation-errors">Redmine API - Validation errors</see>
    /// </summary>
    public class Error
    {
        /// <summary>
        /// List of error infos
        /// </summary>
        public string[] Errors;
    }
}
