using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetNewChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMessageSentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatParticipantStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastReadMessageAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipantStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParticipantStatuses_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Chat_IsRead",
                table: "ChatMessages",
                columns: new[] { "ChatId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Chat_Sender_Read",
                table: "ChatMessages",
                columns: new[] { "ChatId", "SenderType", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Chat_SentAt_Desc",
                table: "ChatMessages",
                columns: new[] { "ChatId", "SentAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Sender",
                table: "ChatMessages",
                columns: new[] { "SenderId", "SenderType" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipantStatus_Participant",
                table: "ChatParticipantStatuses",
                columns: new[] { "ParticipantId", "ParticipantType" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipantStatus_Participant_Online",
                table: "ChatParticipantStatuses",
                columns: new[] { "ParticipantId", "ParticipantType", "IsOnline" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipantStatus_Unique",
                table: "ChatParticipantStatuses",
                columns: new[] { "ChatId", "ParticipantId", "ParticipantType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Doctor_Active_LastMessage",
                table: "Chats",
                columns: new[] { "DoctorId", "IsActive", "LastMessageSentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_DoctorId",
                table: "Chats",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_User_Active_LastMessage",
                table: "Chats",
                columns: new[] { "UserId", "IsActive", "LastMessageSentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_User_Doctor_Active",
                table: "Chats",
                columns: new[] { "UserId", "DoctorId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_UserId",
                table: "Chats",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatParticipantStatuses");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    FromUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ToUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromUserId",
                table: "Messages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ToUserId",
                table: "Messages",
                column: "ToUserId");
        }
    }
}
