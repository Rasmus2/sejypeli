﻿using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class Tasohyppelypeli1 : PhysicsGame
{
    const double nopeus = 200;
    const double hyppyNopeus = 750;
    const int RUUDUN_KOKO = 40;

    static PlatformCharacter pelaaja1;
    PlatformCharacter Ninja;

    FollowerBrain NinjaAivot;

    IntMeter Pisteet;

    Image pelaajanKuva = LoadImage("Ukko");
    Image NinjanKuva = LoadImage("Ukko");
    Image tahtiKuva = LoadImage("tahti");

    SoundEffect maaliAani = LoadSoundEffect("maali");

    public override void Begin()
    {
        Image pelaajanKuva = LoadImage("Ukko");
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();
        LisaaLaskuri();

        

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.5;
        Camera.StayInLevel = true;

    }

    void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('=', LisaaTaso2);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('P', LisaaPelaaja);
        kentta.SetTileMethod('N', LisaaNinja);
        kentta.SetTileMethod('B', LuoLaatikko);
        kentta.SetTileMethod('O', LuoPallo);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.LightBlue, Color.Black);
    }

    void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.DarkGreen;
        Add(taso);
    }

    void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 6.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        AddCollisionHandler<PlatformCharacter,PlatformCharacter>(pelaaja1, "Ninja", TormaaNinjaan);
        Add(pelaaja1);
    }

    void LisaaNinja(Vector paikka, double leveys, double korkeus)
    {
        Ninja = new PlatformCharacter(leveys, korkeus);
        Ninja.Position = paikka;
        Ninja.Mass = 5.0;
        Ninja.Image = NinjanKuva;
        Ninja.Tag = "Ninja";
        Add(Ninja);
        NinjaAivot = new FollowerBrain(pelaaja1);
        NinjaAivot.Speed = 900;
        NinjaAivot.DistanceFar = 600;
        NinjaAivot.Active = true;
        Ninja.Brain = NinjaAivot;
    }



    void LuoLaatikko(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Laatikko = new PhysicsObject(leveys, korkeus);
        Add(Laatikko);
        Laatikko.Position = paikka;
        Laatikko.Color = Color.Brown;
        Laatikko.Restitution = 0.2;
    }

    void LisaaLaskuri()
    {
        Pisteet = LuoPisteLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
    }

    IntMeter LuoPisteLaskuri(double x,double y)
    {
        IntMeter laskuri = new IntMeter(0);
        laskuri.MaxValue = 100;

        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = x;
        naytto.Y = y;
        naytto.TextColor = Color.White;
        Add(naytto);

        return laskuri;
    }

    void LuoPallo(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Pallo = new PhysicsObject(leveys, korkeus);
        Add(Pallo);
        Pallo.Shape = Shape.Circle;
        Pallo.Position = paikka;
        Pallo.Color = Color.Red;
        Pallo.Restitution = 2.0;
    }

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }



    void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    void TormaaNinjaan(PlatformCharacter hahmo, PlatformCharacter Ninja)
    {
        hahmo.Destroy();
        ClearAll();
        Begin();
    }
    
    void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
       Pisteet.Value += 1;
       tahti.Destroy();
    }

    void LisaaTaso2(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso2 = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso2.Position = paikka;
        taso2.Color = Color.Black;
        Add(taso2);
    }
}
