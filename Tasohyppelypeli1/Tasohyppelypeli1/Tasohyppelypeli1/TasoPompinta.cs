using System;
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
    Image NinjanKuva = LoadImage("Ninja");
    Image tahtiKuva = LoadImage("tahti");
    Image tiiliKuva = LoadImage("Tiili");

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
        kentta.SetTileMethod('T', LisaaTukki);
        kentta.SetTileMethod('=', LisaaTaso2);
        kentta.SetTileMethod('_', LisaaTaso3);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('P', LisaaPelaaja);
        kentta.SetTileMethod('N', LisaaNinja);
        kentta.SetTileMethod('B', LuoLaatikko);
        kentta.SetTileMethod('O', LisaaLehti);
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
        NinjaAivot.Speed = 400;
        NinjaAivot.DistanceFar = 500;
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

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        Keyboard.Listen(Key.R, ButtonState.Pressed, retry, "Aloita alusta");

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

    void retry()
    {
        ClearAll();
        Begin();
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
       if (Pisteet == 15)
       {
           ClearAll();
           MessageDisplay.Add("Voitit pelin! Peli ohi!");
           LisaaNappaimet();
       }
    }

    void LisaaTaso2(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso2 = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso2.Position = paikka;
        taso2.Image = tiiliKuva;
        taso2.Color = Color.Red;
        Add(taso2);
    }

    void LisaaTaso3(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso3 = PhysicsObject.CreateStaticObject(leveys, korkeus/2);
        Vector paikka2 = new Vector(paikka.X, paikka.Y - 10);
        taso3.Position = paikka2;
        taso3.Color = Color.DarkGreen;
        Add(taso3);

    }

    void LisaaTukki(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Tukki = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Tukki.Position = paikka;
        Tukki.Color = Color.DarkBrown;
        Tukki.IgnoresCollisionResponse = true;
        Add(Tukki);
        
    }

    void LisaaLehti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Lehti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Lehti.Position = paikka;
        Lehti.Color = Color.Green;
        Lehti.IgnoresCollisionResponse = true;
        Add(Lehti);
    }
}
