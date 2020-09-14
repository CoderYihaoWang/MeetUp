using MeetUp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Identity
{
    public interface IJwtProvider
    {
        string GenerateJwtToken(User user);
    }
}
