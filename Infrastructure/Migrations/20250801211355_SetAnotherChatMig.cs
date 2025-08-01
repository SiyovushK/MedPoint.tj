using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetAnotherChatMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatParticipantStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Doctor_Active_LastMessage",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chat_User_Active_LastMessage",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chat_User_Doctor_Active",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageSentAt",
                table: "Chats");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_DoctorId",
                table: "Chats",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_DoctorId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId",
                table: "Chats");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageSentAt",
                table: "Chats",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastReadMessageAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ParticipantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
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
                name: "IX_Chat_Doctor_Active_LastMessage",
                table: "Chats",
                columns: new[] { "DoctorId", "IsActive", "LastMessageSentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_User_Active_LastMessage",
                table: "Chats",
                columns: new[] { "UserId", "IsActive", "LastMessageSentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_User_Doctor_Active",
                table: "Chats",
                columns: new[] { "UserId", "DoctorId", "IsActive" });

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
                name: "IX_ChatParticipantStatus_Participant_Online",
                table: "ChatParticipantStatuses",
                columns: new[] { "ParticipantId", "ParticipantType", "IsOnline" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipantStatus_Unique",
                table: "ChatParticipantStatuses",
                columns: new[] { "ChatId", "ParticipantId", "ParticipantType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
