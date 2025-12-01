using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi;

public interface IAdmin
{
    void ResetDB();
    void InitializeDB();

    DateTime GetClock();

    void ForwardClock(TimeSpan timeSpan);

    BO.Config? GetConfig();



}
