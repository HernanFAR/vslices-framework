using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Factories;
public class DatabaseFactory
{
    public const string Password = "H4lv.2023IS";

    public static MsSqlContainer BuildContainer()
    {
        return new MsSqlBuilder()
            .WithPassword(Password)
            .WithPortBinding(FreeTcpPort().ToString())
            .Build();
    }

    private static int FreeTcpPort()
    {
        var l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        var port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
}

