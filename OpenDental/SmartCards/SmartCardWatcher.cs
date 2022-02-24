using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using OpenDental.SmartCards.Belgium;
using System.ComponentModel;
using OpenDentBusiness;

namespace OpenDental.SmartCards {
	public class SmartCardWatcher : Component {
		public SmartCardWatcher() {
			manager = SmartCardManager.Load();
			
			// This returns null on operation systems which are not supported. If the current OS is not supported,
			// we don't hook up the event handlers.
			if(manager == null)
				return;
			
			manager.SmartCardChanged += new SmartCardStateChangedEventHandler(OnSmartCardChanged);
			// Register all known Smart Cards
			smartCardServices = new Collection<SmartCardService>();
			smartCardServices.Add(new BelgianIdentityCard(manager));
		}

		private ISmartCardManager manager;
		private Collection<SmartCardService> smartCardServices;

		void OnSmartCardChanged(object sender, SmartCardStateChangedEventArgs e) {
            // If nobody is listening to us, we don't need to do any effort to talk.
            if (PatientCardInserted == null)
                return;

			if(e.State == SmartCardState.Inserted) {
				foreach(SmartCardService service in smartCardServices) {
					if(service.IsSupported(e.Atr)){
                        Patient patient = service.GetPatientInfo(e.Reader);

                        // The event may be unhooked from
                        if (PatientCardInserted != null) {
                            PatientCardInserted(this, new PatientCardInsertedEventArgs(patient));
                        }
					}
				}
			}
		}

        protected override void Dispose(bool disposing)
        {
            manager.Dispose();
            base.Dispose(disposing);
        }

		public event PatientCardInsertedEventHandler PatientCardInserted;
	}
}
