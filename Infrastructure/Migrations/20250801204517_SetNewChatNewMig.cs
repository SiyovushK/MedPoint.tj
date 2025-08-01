using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetNewChatNewMig : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_Chat_DoctorId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chat_UserId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipantStatus_Participant",
                table: "ChatParticipantStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_Sender",
                table: "ChatMessages");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_DoctorId",
                table: "Chats",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipantStatus_Participant",
                table: "ChatParticipantStatuses",
                columns: new[] { "ParticipantId", "ParticipantType" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_Sender",
                table: "ChatMessages",
                columns: new[] { "SenderId", "SenderType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Doctors_DoctorId",
                table: "Chats",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
