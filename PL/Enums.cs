using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

internal class ShipmentType : IEnumerable
{
    static readonly IEnumerable<BO.ShipmentType> s_enums =
        (Enum.GetValues(typeof(BO.ShipmentType)) as IEnumerable<BO.ShipmentType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


internal class OrderStatus : IEnumerable
{
    static readonly IEnumerable<BO.OrderStatus> s_enums =
        (Enum.GetValues(typeof(BO.OrderStatus)) as IEnumerable<BO.OrderStatus>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OrderType : IEnumerable
{
    static readonly IEnumerable<BO.OrderType> s_enums =
        (Enum.GetValues(typeof(BO.OrderType)) as IEnumerable<BO.OrderType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class ScheduleStatus : IEnumerable
{
    static readonly IEnumerable<BO.ScheduleStatus> s_enums =
        (Enum.GetValues(typeof(BO.ScheduleStatus)) as IEnumerable<BO.ScheduleStatus>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public enum OrderOptions
{
    None,
    OrderStatus,
    OrderType,
    ScheduleStatus
}