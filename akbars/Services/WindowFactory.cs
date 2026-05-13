using System.Windows;
using akbars.Models;
using akbars.Views.Admin;
using akbars.Views.Dispatcher;
using akbars.Views.Repairman;
using akbars.Views.Worker;

namespace akbars.Services
{
    public static class WindowFactory
    {
        public static Window CreateDashboard(SessionContext session)
        {
            switch (session.RoleId)
            {
                case 1:
                    return new MainWorker(session);
                case 2:
                    return new MainRepairman(session);
                case 3:
                    return new MainDispatcher(session);
                case 4:
                    return new MainAdmin(session);
                default:
                    return null;
            }
        }
    }
