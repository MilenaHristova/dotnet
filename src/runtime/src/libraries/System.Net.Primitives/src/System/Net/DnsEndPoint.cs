// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace System.Net
{
    public class DnsEndPoint : EndPoint
    {
        private readonly string _host;
        private readonly int _port;
        private readonly AddressFamily _family;

        public DnsEndPoint(string host, int port) : this(host, port, AddressFamily.Unspecified) { }

        public DnsEndPoint(string host, int port, AddressFamily addressFamily)
        {
            ArgumentException.ThrowIfNullOrEmpty(host);

            ArgumentOutOfRangeException.ThrowIfLessThan(port, IPEndPoint.MinPort);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(port, IPEndPoint.MaxPort);

            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6 &&
                addressFamily != AddressFamily.Unspecified)
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, nameof(addressFamily));
            }

            _host = host;
            _port = port;
            _family = addressFamily;
        }

        public override bool Equals([NotNullWhen(true)] object? comparand)
        {
            DnsEndPoint? dnsComparand = comparand as DnsEndPoint;

            if (dnsComparand == null)
            {
                return false;
            }

            return (_family == dnsComparand._family &&
                    _port == dnsComparand._port &&
                    _host == dnsComparand._host);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());
        }

        public override string ToString()
        {
            return _family + "/" + _host + ":" + _port;
        }

        public string Host
        {
            get
            {
                return _host;
            }
        }

        public override AddressFamily AddressFamily
        {
            get
            {
                return _family;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
        }
    }
}
