namespace WaruniAzureAssignement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUserProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileId = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        Telephone = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Profiles");
        }
    }
}
