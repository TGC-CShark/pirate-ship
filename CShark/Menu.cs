using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    class Menu
    {
        TgcSprite fondo = new TgcSprite();
        TgcD3dInput input = GuiController.Instance.D3dInput;
        TgcText2d titulo;
        TgcText2d sombra;

        TgcText2d textoComenzar;


        public Menu(EstadoDelJuego estado)
        {
            fondo.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Barco-Pirata-Menu.jpg");
            fondo.Position = new Vector2(0, 0);
            fondo.Scaling = new Vector2(
                (float)GuiController.Instance.Panel3d.Width / fondo.Texture.Width,
                (float)GuiController.Instance.Panel3d.Height / fondo.Texture.Height);


            titulo = new TgcText2d();
            titulo.Text = "Barco Pirata";
            titulo.Color = Color.Gold;
            titulo.Align = TgcText2d.TextAlign.CENTER;
            titulo.Position = new Point(200, 100);
            titulo.Size = new Size(800, 300);
            titulo.changeFont(new System.Drawing.Font("BlackoakStd", 35, FontStyle.Bold | FontStyle.Italic));

            sombra = new TgcText2d();
            sombra.Text = "Barco Pirata";
            sombra.Color = Color.Chocolate;
            sombra.Align = TgcText2d.TextAlign.CENTER;
            sombra.Position = new Point(205, 103);
            sombra.Size = new Size(800, 300);
            sombra.changeFont(new System.Drawing.Font("BlackoakStd", 35, FontStyle.Bold | FontStyle.Italic));

            textoComenzar = new TgcText2d();
            textoComenzar.Text = "Haga clic para comenzar a jugar";
            textoComenzar.Color = Color.Gold;
            textoComenzar.Align = TgcText2d.TextAlign.CENTER;
            textoComenzar.Position = new Point(450, 250);
            textoComenzar.Size = new Size(300, 100);
            textoComenzar.changeFont(new System.Drawing.Font("BlackoakStd", 18, FontStyle.Bold | FontStyle.Italic));



        }

        public void render(EjemploAlumno juego)
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();
            fondo.render();
            
            GuiController.Instance.Drawer2D.endDrawSprite();

            sombra.render();
            titulo.render();
            textoComenzar.render();
            

            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                juego.estado = EstadoDelJuego.Jugando;
            }

        }
    }
}
