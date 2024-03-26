namespace BookBeat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaLists", "BookID", c => c.Int());
            AddColumn("dbo.MediaLists", "TrackID", c => c.Int());
            CreateIndex("dbo.MediaLists", "BookID");
            CreateIndex("dbo.MediaLists", "TrackID");
            AddForeignKey("dbo.MediaLists", "BookID", "dbo.Books", "BookID");
            AddForeignKey("dbo.MediaLists", "TrackID", "dbo.Tracks", "Id");
            DropColumn("dbo.MediaLists", "MediaID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MediaLists", "MediaID", c => c.Int(nullable: false));
            DropForeignKey("dbo.MediaLists", "TrackID", "dbo.Tracks");
            DropForeignKey("dbo.MediaLists", "BookID", "dbo.Books");
            DropIndex("dbo.MediaLists", new[] { "TrackID" });
            DropIndex("dbo.MediaLists", new[] { "BookID" });
            DropColumn("dbo.MediaLists", "TrackID");
            DropColumn("dbo.MediaLists", "BookID");
        }
    }
}
