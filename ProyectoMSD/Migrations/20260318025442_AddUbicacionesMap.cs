using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoMSD.Migrations
{
    /// <inheritdoc />
    public partial class AddUbicacionesMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "almacenan_ibfk_1",
                table: "almacenan");

            migrationBuilder.DropForeignKey(
                name: "almacenan_ibfk_2",
                table: "almacenan");

            migrationBuilder.DropForeignKey(
                name: "configuraciones_ibfk_2",
                table: "configuraciones");

            migrationBuilder.DropForeignKey(
                name: "propiedades_ibfk_1",
                table: "propiedades");

            migrationBuilder.DropForeignKey(
                name: "soportes_ibfk_1",
                table: "soportes");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "almacenan");

            migrationBuilder.DropIndex(
                name: "ID_Dispositivos",
                table: "almacenan");

            migrationBuilder.DropIndex(
                name: "ID_Espacios",
                table: "almacenan");

            migrationBuilder.RenameTable(
                name: "almacenan",
                newName: "Almacenan");

            migrationBuilder.RenameIndex(
                name: "ID_Dispositivos1",
                table: "configuraciones",
                newName: "ID_Dispositivos");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Almacenan",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID_Espacios",
                table: "Almacenan",
                newName: "IdEspacios");

            migrationBuilder.RenameColumn(
                name: "ID_Dispositivos",
                table: "Almacenan",
                newName: "IdDispositivos");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "usuarios",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(int),
                oldType: "int(11)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ID_Ubicacion",
                table: "usuarios",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrefijoTelefono",
                table: "usuarios",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true,
                collation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IdUsuariosNavigationId",
                table: "soportes",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuariosNavigationId",
                table: "propiedades",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UbicacioneId",
                table: "propiedades",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UbicacioneId",
                table: "espacios",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Usos",
                table: "dispositivos",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "dispositivos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "dispositivos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "dispositivos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "dispositivos",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "ID_Espacio",
                table: "dispositivos",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MAC_Address",
                table: "dispositivos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Protocolo",
                table: "dispositivos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IdUsuariosNavigationId",
                table: "configuraciones",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Almacenan",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdEspacios",
                table: "Almacenan",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "IdDispositivos",
                table: "Almacenan",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AddColumn<int>(
                name: "IdDispositivosNavigationId",
                table: "Almacenan",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEspaciosNavigationId",
                table: "Almacenan",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Almacenan",
                table: "Almacenan",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdUsuarios = table.Column<int>(type: "int(11)", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mensaje = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Leida = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    RutaRedireccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificaciones_usuarios",
                        column: x => x.IdUsuarios,
                        principalTable: "usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "ubicaciones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Latitud = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    DireccionFormateada = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "usuarios_propiedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_Usuario = table.Column<int>(type: "int(11)", nullable: false),
                    ID_Propiedad = table.Column<int>(type: "int(11)", nullable: false),
                    RolEnPropiedad = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUsuarioNavigationId = table.Column<int>(type: "int(11)", nullable: false),
                    IdPropiedadNavigationId = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_propiedades_propiedades_IdPropiedadNavigationId",
                        column: x => x.IdPropiedadNavigationId,
                        principalTable: "propiedades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_propiedades_usuarios_IdUsuarioNavigationId",
                        column: x => x.IdUsuarioNavigationId,
                        principalTable: "usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_ID_Ubicacion",
                table: "usuarios",
                column: "ID_Ubicacion");

            migrationBuilder.CreateIndex(
                name: "IX_soportes_IdUsuariosNavigationId",
                table: "soportes",
                column: "IdUsuariosNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_propiedades_IdUsuariosNavigationId",
                table: "propiedades",
                column: "IdUsuariosNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_propiedades_UbicacioneId",
                table: "propiedades",
                column: "UbicacioneId");

            migrationBuilder.CreateIndex(
                name: "IX_espacios_UbicacioneId",
                table: "espacios",
                column: "UbicacioneId");

            migrationBuilder.CreateIndex(
                name: "IX_dispositivos_ID_Espacio",
                table: "dispositivos",
                column: "ID_Espacio");

            migrationBuilder.CreateIndex(
                name: "IX_configuraciones_IdUsuariosNavigationId",
                table: "configuraciones",
                column: "IdUsuariosNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Almacenan_IdDispositivosNavigationId",
                table: "Almacenan",
                column: "IdDispositivosNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Almacenan_IdEspaciosNavigationId",
                table: "Almacenan",
                column: "IdEspaciosNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_IdUsuarios",
                table: "notificaciones",
                column: "IdUsuarios");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_propiedades_IdPropiedadNavigationId",
                table: "usuarios_propiedades",
                column: "IdPropiedadNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_propiedades_IdUsuarioNavigationId",
                table: "usuarios_propiedades",
                column: "IdUsuarioNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Almacenan_dispositivos_IdDispositivosNavigationId",
                table: "Almacenan",
                column: "IdDispositivosNavigationId",
                principalTable: "dispositivos",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Almacenan_espacios_IdEspaciosNavigationId",
                table: "Almacenan",
                column: "IdEspaciosNavigationId",
                principalTable: "espacios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_configuraciones_usuarios_IdUsuariosNavigationId",
                table: "configuraciones",
                column: "IdUsuariosNavigationId",
                principalTable: "usuarios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dispositivos_espacios_ID_Espacio",
                table: "dispositivos",
                column: "ID_Espacio",
                principalTable: "espacios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_espacios_ubicaciones_UbicacioneId",
                table: "espacios",
                column: "UbicacioneId",
                principalTable: "ubicaciones",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_propiedades_ubicaciones_UbicacioneId",
                table: "propiedades",
                column: "UbicacioneId",
                principalTable: "ubicaciones",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_propiedades_usuarios_IdUsuariosNavigationId",
                table: "propiedades",
                column: "IdUsuariosNavigationId",
                principalTable: "usuarios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_soportes_usuarios_IdUsuariosNavigationId",
                table: "soportes",
                column: "IdUsuariosNavigationId",
                principalTable: "usuarios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_ubicaciones",
                table: "usuarios",
                column: "ID_Ubicacion",
                principalTable: "ubicaciones",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Almacenan_dispositivos_IdDispositivosNavigationId",
                table: "Almacenan");

            migrationBuilder.DropForeignKey(
                name: "FK_Almacenan_espacios_IdEspaciosNavigationId",
                table: "Almacenan");

            migrationBuilder.DropForeignKey(
                name: "FK_configuraciones_usuarios_IdUsuariosNavigationId",
                table: "configuraciones");

            migrationBuilder.DropForeignKey(
                name: "FK_dispositivos_espacios_ID_Espacio",
                table: "dispositivos");

            migrationBuilder.DropForeignKey(
                name: "FK_espacios_ubicaciones_UbicacioneId",
                table: "espacios");

            migrationBuilder.DropForeignKey(
                name: "FK_propiedades_ubicaciones_UbicacioneId",
                table: "propiedades");

            migrationBuilder.DropForeignKey(
                name: "FK_propiedades_usuarios_IdUsuariosNavigationId",
                table: "propiedades");

            migrationBuilder.DropForeignKey(
                name: "FK_soportes_usuarios_IdUsuariosNavigationId",
                table: "soportes");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_ubicaciones",
                table: "usuarios");

            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "ubicaciones");

            migrationBuilder.DropTable(
                name: "usuarios_propiedades");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_ID_Ubicacion",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_soportes_IdUsuariosNavigationId",
                table: "soportes");

            migrationBuilder.DropIndex(
                name: "IX_propiedades_IdUsuariosNavigationId",
                table: "propiedades");

            migrationBuilder.DropIndex(
                name: "IX_propiedades_UbicacioneId",
                table: "propiedades");

            migrationBuilder.DropIndex(
                name: "IX_espacios_UbicacioneId",
                table: "espacios");

            migrationBuilder.DropIndex(
                name: "IX_dispositivos_ID_Espacio",
                table: "dispositivos");

            migrationBuilder.DropIndex(
                name: "IX_configuraciones_IdUsuariosNavigationId",
                table: "configuraciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Almacenan",
                table: "Almacenan");

            migrationBuilder.DropIndex(
                name: "IX_Almacenan_IdDispositivosNavigationId",
                table: "Almacenan");

            migrationBuilder.DropIndex(
                name: "IX_Almacenan_IdEspaciosNavigationId",
                table: "Almacenan");

            migrationBuilder.DropColumn(
                name: "ID_Ubicacion",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "PrefijoTelefono",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "IdUsuariosNavigationId",
                table: "soportes");

            migrationBuilder.DropColumn(
                name: "IdUsuariosNavigationId",
                table: "propiedades");

            migrationBuilder.DropColumn(
                name: "UbicacioneId",
                table: "propiedades");

            migrationBuilder.DropColumn(
                name: "UbicacioneId",
                table: "espacios");

            migrationBuilder.DropColumn(
                name: "ID_Espacio",
                table: "dispositivos");

            migrationBuilder.DropColumn(
                name: "MAC_Address",
                table: "dispositivos");

            migrationBuilder.DropColumn(
                name: "Protocolo",
                table: "dispositivos");

            migrationBuilder.DropColumn(
                name: "IdUsuariosNavigationId",
                table: "configuraciones");

            migrationBuilder.DropColumn(
                name: "IdDispositivosNavigationId",
                table: "Almacenan");

            migrationBuilder.DropColumn(
                name: "IdEspaciosNavigationId",
                table: "Almacenan");

            migrationBuilder.RenameTable(
                name: "Almacenan",
                newName: "almacenan");

            migrationBuilder.RenameIndex(
                name: "ID_Dispositivos",
                table: "configuraciones",
                newName: "ID_Dispositivos1");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "almacenan",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "IdEspacios",
                table: "almacenan",
                newName: "ID_Espacios");

            migrationBuilder.RenameColumn(
                name: "IdDispositivos",
                table: "almacenan",
                newName: "ID_Dispositivos");

            migrationBuilder.AlterColumn<int>(
                name: "Telefono",
                table: "usuarios",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.UpdateData(
                table: "dispositivos",
                keyColumn: "Usos",
                keyValue: null,
                column: "Usos",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Usos",
                table: "dispositivos",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "dispositivos",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "dispositivos",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "dispositivos",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "dispositivos",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "almacenan",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ID_Espacios",
                table: "almacenan",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID_Dispositivos",
                table: "almacenan",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "almacenan",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "ID_Dispositivos",
                table: "almacenan",
                column: "ID_Dispositivos");

            migrationBuilder.CreateIndex(
                name: "ID_Espacios",
                table: "almacenan",
                column: "ID_Espacios");

            migrationBuilder.AddForeignKey(
                name: "almacenan_ibfk_1",
                table: "almacenan",
                column: "ID_Espacios",
                principalTable: "espacios",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "almacenan_ibfk_2",
                table: "almacenan",
                column: "ID_Dispositivos",
                principalTable: "dispositivos",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "configuraciones_ibfk_2",
                table: "configuraciones",
                column: "ID_Usuarios",
                principalTable: "usuarios",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "propiedades_ibfk_1",
                table: "propiedades",
                column: "ID_USUARIOS",
                principalTable: "usuarios",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "soportes_ibfk_1",
                table: "soportes",
                column: "ID_USUARIOS",
                principalTable: "usuarios",
                principalColumn: "ID");
        }
    }
}
