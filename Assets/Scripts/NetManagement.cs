using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetManagement
{
    public static uint ToUInt32(IP ip)
    {
        return ((uint)ip.GetIPAsBytes()[0] << 24)
            | ((uint)ip.GetIPAsBytes()[1] << 16)
            | ((uint)ip.GetIPAsBytes()[2] << 8)
            | ip.GetIPAsBytes()[3];
    }

    public static string UInt32ToIPString(uint ip)
    {
        return $"{(ip >> 24) & 0xFF}.{(ip >> 16) & 0xFF}.{(ip >> 8) & 0xFF}.{ip & 0xFF}";
    }

    public static uint ToUInt32(Mask mask)
    {
        return ((uint)mask.GetMaskAsBytes()[0] << 24)
            | ((uint)mask.GetMaskAsBytes()[1] << 16)
            | ((uint)mask.GetMaskAsBytes()[2] << 8)
            | mask.GetMaskAsBytes()[3];
    }

    private static byte[] ConvertUInt32ToBytes(uint value)
    {
        return new byte[]
        {
            (byte)((value >> 24) & 0xFF),
            (byte)((value >> 16) & 0xFF),
            (byte)((value >> 8) & 0xFF),
            (byte)(value & 0xFF),
        };
    }

    public static bool IsBetween(IP ipToCheck, IP startIP, IP endIP)
    {
        uint ipVal = ipToCheck.GetIPAsUInt32();
        uint startVal = startIP.GetIPAsUInt32();
        uint endVal = endIP.GetIPAsUInt32();

        return ipVal >= startVal && ipVal <= endVal;
    }

    public static bool IsInSubnet(IP ipToCheck, IP netIP, Mask mask)
    {
        uint ipVal = ipToCheck.GetIPAsUInt32();
        uint networkVal = netIP.GetIPAsUInt32();
        uint maskVal = mask.GetMaskAsUInt32();
        return (ipVal & maskVal) == (networkVal & maskVal);
    }

    public Network CombineNetworks(Network first, Network second)
    {
        // Ensure both networks have the same mask
        if (
            first.GetMask().GetMaskAsStringDottedNotation()
            != second.GetMask().GetMaskAsStringDottedNotation()
        )
        {
            throw new System.Exception(
                "Networks cannot be combined if they have different subnet masks."
            );
        }

        // Get the parent network of the two by comparing their IPs
        Network parent = GetParentNetwork(first, second);

        // Combine tags from both networks
        List<Tag> combinedTags = new List<Tag>();
        combinedTags.AddRange(first.GetTags());
        combinedTags.AddRange(second.GetTags());

        // Create the combined network
        Network combinedNetwork = new Network(
            $"{first.GetName()}_{second.GetName()}",
            parent.GetIP(),
            parent.GetMask()
        );

        // Add combined tags to the new network
        foreach (var tag in combinedTags)
        {
            combinedNetwork.AddTag(tag);
        }

        return combinedNetwork;
    }

    private Network GetParentNetwork(Network first, Network second)
    {
        // Identify the broader parent network of the two
        string firstIPBinary = ConvertToBinary(first.GetIP());
        string secondIPBinary = ConvertToBinary(second.GetIP());
        Mask mask = first.GetMask();

        string subnet = firstIPBinary.Substring(0, mask.GetMaskBitsCount());
        string otherSubnet = secondIPBinary.Substring(0, mask.GetMaskBitsCount());

        if (subnet == otherSubnet)
        {
            // The broader network is the one whose IP is the same until the mask's number of bits
            return first; // or second, since they satisfy the same range
        }

        throw new System.Exception("Networks are not part of the same superclass.");
    }

    private string ConvertToBinary(IP ip)
    {
        return string.Join(
            "",
            ip.GetIPAsBytes().Select(x => Convert.ToString(x, 2).PadLeft(8, '0'))
        );
    }

    public static List<Network> SortNetworks(List<Network> networks)
    {
        return networks
            .OrderBy(n => n.GetIP().GetIPAsUInt32())
            .ThenBy(n => n.GetMask().GetMaskAsUInt32())
            .ToList();
    }

    public static List<Network> SubnetNetwork(Network network, Mask newMask)
    {
        // Validate that the new mask is larger (smaller subnet size)
        if (newMask.GetMaskBitsCount() <= network.GetMask().GetMaskBitsCount())
        {
            throw new System.Exception(
                $"New mask must be larger than the current network mask to create subnets.\nCurrent Mask: {network.GetMask().GetMaskAsStringDottedNotation()}, New Mask: {newMask.GetMaskAsStringDottedNotation()}"
            );
        }

        // Get the starting IP of the parent network
        IP netIP = network.GetIP();
        List<Network> subnets = new List<Network>();

        int oldMaskBits = network.GetMask().GetMaskBitsCount();
        int newMaskBits = newMask.GetMaskBitsCount();

        // Calculate the number of subnets that will be created
        int numberOfSubnets = (int)Math.Pow(2, newMaskBits - oldMaskBits);

        for (int i = 0; i < numberOfSubnets; i++)
        {
            string subnetName = $"{network.GetName()}_{i + 1}";
            Network subnet = new Network(subnetName, netIP, newMask);
            subnet.AddTagRange(GetTagsInSubnet(network, netIP, newMask));
            subnets.Add(subnet);

            // Move to the next subnet's starting IP
            if (i < numberOfSubnets - 1)
                netIP = GetNextSubnetStartIP(netIP, newMask);
        }

        return subnets;
    }

    private static List<Tag> GetTagsInSubnet(Network network, IP netIP, Mask mask)
    {
        List<Tag> tagsInSubnet = new List<Tag>();
        List<Tag> tags = network.GetTags();
        foreach (Tag tag in tags)
        {
            if (IsInSubnet(tag.ip, netIP, mask))
            {
                tagsInSubnet.Add(tag);
            }
        }

        return tagsInSubnet;
    }

    private static IP GetNextSubnetStartIP(IP currentIP, Mask subnetMask)
    {
        // Get the current IP as a 32-bit integer
        uint currentIPAsUInt32 = currentIP.GetIPAsUInt32();
        uint subnetIncrement = (~subnetMask.GetMaskAsUInt32()) + 1; // Calculate size of subnet

        // Calculate the next subnet's starting IP
        uint nextIPAsUInt32 = currentIPAsUInt32 + subnetIncrement;
        byte[] nextIPBytes = ConvertUInt32ToBytes(nextIPAsUInt32);
        return new IP(nextIPBytes);
    }

    public static void ApplyNetChanges(
        List<Network> networks_toDelete,
        List<Network> networks_toAdd
    )
    {
        NetworkData networkData = NetHolder.GetNetworkData();

        foreach (Network net in networks_toDelete)
        {
            networkData.RemoveNetworkBase(net);
        }

        foreach (Network net in networks_toAdd)
        {
            networkData.AddNetworkBase(net);
        }
    }

    public static bool ValidateByteAsString(string ipByteString)
    {
        if (!byte.TryParse(ipByteString, out byte byteSegment))
            return false;

        return true;
    }

    static readonly byte[] validMaskBytes = new byte[9]
    {
        0,
        128,
        192,
        224,
        240,
        248,
        252,
        254,
        255,
    };

    public static bool ValidateMaskByte(byte bite)
    {
        return validMaskBytes.Contains(bite);
    }

    public static uint[] GetNetworkIds(IP sourceIp, Mask sourceMask)
    {
        uint ipVal = ToUInt32(sourceIp);
        uint maskVal = ToUInt32(sourceMask);
        uint step = ~maskVal + 1;
        if (step == 0)
            return new uint[] { 0 };

        uint currentNetworkId = ipVal & maskVal;
        uint startPoint = (ipVal <= currentNetworkId) ? currentNetworkId : currentNetworkId + step;

        // RACJONALNY LIMIT:
        // Jeśli maska jest ciasna (zmienia tylko końcówkę), trzymaj się oktetu (255 adresów).
        // Jeśli maska jest szeroka, pozwól pętli na 100 wyników bez ograniczeń oktetowych.
        uint limit;
        if (maskVal >= 0xFFFFFF00) // Maski /24 i wyższe
        {
            limit = (ipVal & 0xFFFFFF00) | 0x000000FF;
        }
        else if (maskVal >= 0xFFFF0000) // Maski /16 do /23
        {
            limit = (ipVal & 0xFF000000) | 0x00FFFFFF;
        }
        else if (maskVal >= 0xFF000000) // Maski /8 do /15
        {
            limit = (ipVal & 0xFFFF0000) | 0x0000FFFF;
        }
        else if (maskVal >= 0x00000000) // Maski /0 do /7
        {
            limit = (ipVal & 0x00000000) | 0xFFFFFFFF;
        }
        else
        {
            limit = uint.MaxValue;
        }

        List<uint> result = new List<uint>();
        uint current = startPoint;

        while (result.Count < 100 && current <= limit)
        {
            result.Add(current);
            if (uint.MaxValue - current < step)
                break;
            current += step;
        }

        return result.ToArray();
    }

    public static void AddNetwork(Network net)
    {
        NetworkData networkData = NetHolder.GetNetworkData();
        networkData.AddNetworkBase(net);
    }

    public static Network GetNetworkByName(string name)
    {
        NetworkData networkData = NetHolder.GetNetworkData();
        return networkData.GetNetworkBases().FirstOrDefault(n => n.GetName() == name);
    }

    public static void RemoveNetwork(Network net)
    {
        NetworkData networkData = NetHolder.GetNetworkData();
        networkData.RemoveNetworkBase(net);
    }
}
