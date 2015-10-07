using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class BarraVida
    {
        public TgcSprite sprite;
        public TgcText2d nombre;

        public BarraVida(Vector2 position, string nombre)
        {
            this.nombre = new TgcText2d();
            this.nombre.Text = nombre;
            this.nombre.Align = TgcText2d.TextAlign.LEFT;
            this.nombre.changeFont(new System.Drawing.Font("Tahoma", 17));
            crearSprite();
            posicionar(position);
            escalar(1);
        }

        private void crearSprite()
        {
            this.sprite = new TgcSprite();
            this.sprite.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "barra.jpg");
        }

        public void posicionar(Vector2 position)
        {
            sprite.Position = position + new Vector2(0, 20);
            nombre.Position = new Point((int)position.X, (int)position.Y);
        }

        public void escalar(float escala)
        {
            sprite.Scaling = new Vector2(escalaInicial() * escala, escalaInicial());
        }

        public float escalaInicial()
        {
            return 0.04f;
        }

        public void render()
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();
            nombre.render();
            sprite.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose()
        {
            sprite.dispose();
        }

        public void alinearDerecha()
        {
            this.nombre.Position = new Point((int)(GuiController.Instance.Panel3d.Size.Width - anchoInicial()), (int)nombre.Position.Y);
            this.sprite.Position = new Vector2(GuiController.Instance.Panel3d.Size.Width - anchoInicial(), sprite.Position.Y);
        }

        public float anchoInicial()
        {
            return escalaInicial() * this.sprite.Texture.Width;
        }
    }

    public class BarraVidaEnemigo : BarraVida { 
        public BarraVidaEnemigo(Vector2 position, string nombre) : base(position, nombre)
        {
            this.nombre.Align = TgcText2d.TextAlign.RIGHT;
        }
        public void posicionar(Vector2 position)
        {
            sprite.Position = position + new Vector2(500, 20);
            nombre.Position = new Point((int)position.X, (int)position.Y);
        }
    }
}
