using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GravataOnlineAuth.Attributes
{
    /// <summary>
    /// The permission attribute holds the KEY you have created in the auth API.
    /// If you have created a key, you can use it to validate the token with the given permission.
    /// Please note that the key name is case sensitive, and must match exactly the key you have created in the auth API.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PermissionAttribute : Attribute
    {
        private string name;
        /// <summary>
        /// The permission attribute holds the KEY you have created in the auth API.
        /// If you have created a key, you can use it to validate the token with the given permission.
        /// Please note that the key name is case sensitive, and must match exactly the key you have created in the auth API.
        /// </summary>
        public PermissionAttribute(string name)
        {
            this.name = name;

        }


        /// <summary>
        /// The key name you have created in the auth API.
        /// </summary>
        public virtual string Name
        {
            get { return name; }
        }


    }
}
