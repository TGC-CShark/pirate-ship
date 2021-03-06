﻿using Microsoft.DirectX;
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

        TgcText2d textoComplementario;


        public Menu()
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

            textoComplementario = new TgcText2d();
            textoComplementario.Text = "Hacé clic para comenzar a jugar." + '\n' + "Apretá I durante el juego para ver las instrucciones.";
            textoComplementario.Color = Color.Gold;
            textoComplementario.Align = TgcText2d.TextAlign.CENTER;
            textoComplementario.Position = new Point(450, 250);
            textoComplementario.Size = new Size(300, 100);
            textoComplementario.changeFont(new System.Drawing.Font("BlackoakStd", 18, FontStyle.Bold | FontStyle.Italic));



        }

        public void render(EjemploAlumno juego)
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();
            fondo.render();

            GuiController.Instance.Drawer2D.endDrawSprite();

            sombra.render();
            titulo.render();
        }

        public void renderSinEmpezar(EjemploAlumno juego)
        {
            render(juego);
            textoComplementario.render();

            jugar(juego);

        }

        public void renderInstrucciones(EjemploAlumno juego)
        {
            titulo.Text = "INSTRUCCIONES";
            sombra.Text = "INSTRUCCIONES";

            textoComplementario.Position = new Point(230, 250);
            textoComplementario.Size = new Size(700, 100);
            textoComplementario.Text = "Dispararle al barco enemigo [SPACE] hasta hundirlo. Con las flechitas RIGHT y LEFT vira el barco. Con UP se acelera y DOWN desacelera." + '\n' +
                "Con A se activa la visibilidad del ángulo de elevación del disparo, y con W y S se incrementa o decrementa dicho ángulo." + '\n' +
                "Con L se activa la tormenta." + '\n' + '\n' +
                "Hacé clic para continuar.";

            render(juego);
            textoComplementario.render();

            jugar(juego);
        }

        public void renderGanado(EjemploAlumno juego)
        {
            titulo.Text = "¡GANASTE!";
            sombra.Text = "¡GANASTE!";

            render(juego);
            
        }

        public void renderPerdido(EjemploAlumno juego)
        {
            titulo.Text = "¡PERDISTE!";
            sombra.Text = "¡PERDISTE!";

            render(juego);

            
        }

        private void jugar(EjemploAlumno juego)
        {
            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {

                juego.estado = EstadoDelJuego.Jugando;
            }
        }


    }
}
