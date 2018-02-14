using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using Server.Configuration;
using Server.MasterData.Model;
using Server.State;
using Server.Util;

namespace Server
{
    public class Global : HttpApplication
    {
        protected internal static SiteState SiteState;
        protected static string DbConnection;
        protected internal static UserSessionContainer UserContainer;

        protected void Application_Start(object sender, EventArgs e)
        {
            var config = new DatabaseConfiguration("NopePets");

            var container = new Container(config);
            UserContainer = new UserSessionContainer(container, config);

            SiteState = SiteState.Initialise(config, 
                container.SiteRepository<User, UserPet>(),
                container.SiteRepository<Pet, PetMetric>(), 
                container.SiteRepository<Animal, AnimalMetric>(),
                container.SiteRepository<Interaction, MetricInteraction>(),
                container.SiteRepository<Metric, MetricInteraction>(), 
                container.DataProvider<User>(), 
                container.SiteRequestProcessor(), 
                UserContainer);

            RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(SiteService)));
        }

        protected void Session_Start(object sender, EventArgs e)
        {
          
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}