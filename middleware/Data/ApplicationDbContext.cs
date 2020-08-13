using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace middleware.Data
{
    public class MWDBContext : IdentityDbContext
    {
        public MWDBContext(DbContextOptions<MWDBContext> options)
            : base(options)
        {
        }
    }
}
