using Microsoft.EntityFrameworkCore;
using ServerDatabaseSystem.DbModels;

namespace ServerDatabaseSystem
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-BIJFUOL;
                Initial Catalog=ChatServerDatabase;Integrated Security=True;MultipleActiveResultSets=True;");
            }
            //optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RelationChatUser> RelationChatUsers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
    }
}
