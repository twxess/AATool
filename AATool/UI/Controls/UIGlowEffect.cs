﻿using System.Xml;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIGlowEffect : UIPicture
    {
        public float Brightness { get; set; }
        public float Scale { get; set; }

        private float displayBrightness;
        private float rotationFactor = 400f;

        private bool isMainWindow;

        public UIGlowEffect()
        {
            this.Layer = Layer.Glow;
        }

        public void SetRotationSpeed(float factor) => this.rotationFactor = MathHelper.Max(factor, 1);

        public void LerpToBrightness(float brightness) => this.Brightness = MathHelper.Clamp(brightness, 0, 1);

        public void SkipToBrightness(float brightness)
        {
            this.Brightness = brightness;
            this.displayBrightness = this.Brightness;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.isMainWindow = screen is UIMainScreen;
            this.displayBrightness = this.Brightness;
        }

        protected override void UpdateThis(Time time)
        {
            float offset = this.isMainWindow && this.Parent is UIObjectiveFrame
                ? (this.X * 100) + (this.Y * 100)
                : 0;

            this.SetRotation((float)(offset + (time.TotalFrames / this.rotationFactor)));
            this.displayBrightness = MathHelper.Lerp(this.displayBrightness, this.Brightness, (float)(10 * time.Delta));
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.Draw(
                this.Texture, 
                this.Inner.Center.ToVector2(), 
                this.Rotation, 
                new Vector2(this.Scale), 
                this.Tint * this.displayBrightness, 
                this.Layer);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.LerpToBrightness(Attribute(node, "brightness", 1f));
            this.Scale = Attribute(node, "scale", 1f);
            this.rotationFactor = Attribute(node, "rotation_speed", 400f);
            this.Layer = Attribute(node, "layer", Layer.Glow);
        }
    }
}
