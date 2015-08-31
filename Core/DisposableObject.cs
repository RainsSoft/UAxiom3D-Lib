#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Linq;

#endregion Namespace Declarations

namespace Axiom.Core
{
    public abstract class DisposableObject : IDisposable {
        //[AxiomHelper(0, 9)]
        protected DisposableObject() {
            IsDisposed = false;
#if DEBUG
            var stackTrace = string.Empty;
#if !(SILVERLIGHT || XBOX || XBOX360 || WINDOWS_PHONE || ANDROID) && AXIOM_ENABLE_LOG_STACKTRACE
			stackTrace = Environment.StackTrace;
#endif
            //ObjectManager.Instance.Add(this, stackTrace);
#endif
        }

        //[AxiomHelper(0, 9)]
        ~DisposableObject() {
            if (!IsDisposed) {
                dispose(false);
            }
        }

        #region IDisposable Implementation

        /// <summary>
        /// Determines if this instance has been disposed of already.
        /// </summary>
        //[AxiomHelper(0, 9)]
        public bool IsDisposed { get; set; }

        /// <summary>
        /// Class level dispose method
        /// </summary>
        /// <remarks>
        /// When implementing this method in an inherited class the following template should be used;
        /// protected override void dispose( bool disposeManagedResources )
        /// {
        /// 	if ( !IsDisposed )
        /// 	{
        /// 		if ( disposeManagedResources )
        /// 		{
        /// 			// Dispose managed resources.
        /// 		}
        ///
        /// 		// There are no unmanaged resources to release, but
        /// 		// if we add them, they need to be released here.
        /// 	}
        ///
        /// 	// If it is available, make the call to the
        /// 	// base class's Dispose(Boolean) method
        /// 	base.dispose( disposeManagedResources );
        /// }
        /// </remarks>
        /// <param name="disposeManagedResources">True if Unmanaged resources should be released.</param>
        //[AxiomHelper(0, 9)]
        protected virtual void dispose(bool disposeManagedResources) {
            if (!IsDisposed) {
                if (disposeManagedResources) {
                    // Dispose managed resources.
#if DEBUG
                   // ObjectManager.Instance.Remove(this);
#endif
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            IsDisposed = true;
        }

        //[AxiomHelper(0, 9)]
        public void Dispose() {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation
    };
}
