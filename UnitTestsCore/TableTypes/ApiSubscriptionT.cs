using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
  public class ApiSubscriptionT {

    ///<summary>Creates a new WatchTable type ApiSubscription. CustomerKey defaults to: "VzkmZEaUWOjnQX2z". WatchTable defaults to Patient.</summary>
    public static ApiSubscription CreateApiSubscriptionWatchTableType(string endPointUrl,DateTime dateTimeStart,DateTime dateTimeStop,string workstation="",
      string customerKey="VzkmZEaUWOjnQX2z",int pollingSeconds=5,EnumWatchTable watchTable=EnumWatchTable.Patient,string note="")
    {
      ApiSubscription apiSubscription=new ApiSubscription();
      apiSubscription.EndPointUrl=endPointUrl;
      apiSubscription.Workstation=workstation;
      apiSubscription.CustomerKey=customerKey;
      apiSubscription.PollingSeconds=pollingSeconds;
      apiSubscription.WatchTable=watchTable.ToString();
      apiSubscription.DateTimeStart=dateTimeStart;
      apiSubscription.DateTimeStop=dateTimeStop;
      apiSubscription.Note=note;
      ApiSubscriptions.Insert(apiSubscription);
      ApiSubscriptions.RefreshCache();
      return apiSubscription;
    }

    ///<summary>Creates a new UiEventType ApiSubscription. CustomerKey defaults to: "VzkmZEaUWOjnQX2z". UiEventType defaults to PatientSelected.</summary>
    public static ApiSubscription CreateApiSubscriptionUiEventType(string endPointUrl,DateTime dateTimeStart,DateTime dateTimeStop,string workstation="",
      string customerKey="VzkmZEaUWOjnQX2z",EnumApiUiEventType uiEventType=EnumApiUiEventType.PatientSelected,string note="")
    {
      ApiSubscription apiSubscription=new ApiSubscription();
      apiSubscription.EndPointUrl=endPointUrl;
      apiSubscription.Workstation=workstation;
      apiSubscription.CustomerKey=customerKey;
      apiSubscription.PollingSeconds=0;//PollingSeconds must be 0 for UiEventType.
      apiSubscription.UiEventType=uiEventType.ToString();
      apiSubscription.DateTimeStart=dateTimeStart;
      apiSubscription.DateTimeStop=dateTimeStop;
      apiSubscription.Note=note;
      ApiSubscriptions.Insert(apiSubscription);
      ApiSubscriptions.RefreshCache();
      return apiSubscription;
    }

    ///<summary>Deletes everything from the apisubscription table.</summary>
    public static void ClearApiSubscriptionTable() {
      string command="DELETE FROM apisubscription";
      DataCore.NonQ(command);
    }

  }

}
