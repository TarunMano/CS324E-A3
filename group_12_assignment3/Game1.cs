using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_12_assignment3;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    private List<string> _uniqueWords;
    private List<string> _displayWords;
    private List<Color> _wordColors;
    private List<(int freq, int count)> _freqData;
    private Texture2D _pixel;
    
    private Color _deepBlue = new Color(25, 72, 145);
    private Color _crimsonRed = new Color(178, 34, 52);
    private Color _goldenYellow = new Color(218, 165, 32);
    private KeyboardState _prevKeyboardState;
    private bool _showUniqueWords;

    private const int CANVAS_WIDTH = 700;
    private const int CANVAS_HEIGHT = 600;
    private const int FONT_SIZE = 20;
    private const int WORD_SPACING = 10;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = CANVAS_WIDTH;
        _graphics.PreferredBackBufferHeight = CANVAS_HEIGHT;
    }

    protected override void Initialize()
    {
        _uniqueWords = new List<string>();
        _displayWords = new List<string>();

        using (var stream = TitleContainer.OpenStream("Content/uniquewords.txt"))
        using (var reader = new StreamReader(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _uniqueWords.Add(line.Trim());
            }
        }

        _freqData = new List<(int freq, int count)>();
        using (var stream = TitleContainer.OpenStream("Content/wordfrequency.txt"))
        using (var reader = new StreamReader(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0) continue;

                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                if (int.TryParse(parts[0].Trim(), out int freq) &&
                    int.TryParse(parts[1].Trim(), out int count))
                {
                    _freqData.Add((freq, count));
                }
            }
        }

        _showUniqueWords = true;

        base.Initialize();
    }

    private void GenerateRandomWords()
    {
        _displayWords.Clear();

        Random rand = new Random();

        // shuffle words
        List<string> shuffled = new List<string>(_uniqueWords);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            string temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        float currentX = WORD_SPACING;
        float currentY = WORD_SPACING;

        foreach (string word in shuffled)
        {
            Vector2 wordSize = _font.MeasureString(word);

            // check if word fits on current line
            if (currentX + wordSize.X + WORD_SPACING > CANVAS_WIDTH)
            {
                currentX = WORD_SPACING;
                currentY += wordSize.Y + WORD_SPACING;
            }

            // check if out of vertical space
            if (currentY + wordSize.Y > CANVAS_HEIGHT)
            {
                break;
            }

            _displayWords.Add(word);

            currentX += wordSize.X + WORD_SPACING;
        }
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("fonts/Avocedit Smoothie");

        GenerateRandomWords();
        
        _wordColors = new List<Color>();
        Random rand = new Random();
        Color[] availableColors = { _deepBlue, _crimsonRed, _goldenYellow };
    
        for (int i = 0; i < _displayWords.Count; i++)
        {
            _wordColors.Add(availableColors[rand.Next(3)]);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState currentKeyboardState = Keyboard.GetState();
        
        // Check for Enter key press (not held)
        if (currentKeyboardState.IsKeyDown(Keys.Enter) && _prevKeyboardState.IsKeyUp(Keys.Enter))
        {
            _showUniqueWords = !_showUniqueWords;
        }
        
        _prevKeyboardState = currentKeyboardState;

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        
        _spriteBatch.Begin();
        
        if (_showUniqueWords)
        {
            DisplayUniqueWords();
        }
        else
        {
            DisplayWordFrequency();
        }
        
        _spriteBatch.End();
        
        base.Draw(gameTime);

    }

    private void DisplayUniqueWords()
    {
        float currentX = WORD_SPACING;
        float currentY = WORD_SPACING;

        for (int i = 0; i < _displayWords.Count; i++)
        {
            string word = _displayWords[i];
            Vector2 wordSize = _font.MeasureString(word);

            if (currentX + wordSize.X + WORD_SPACING > CANVAS_WIDTH)
            {
                currentX = WORD_SPACING;
                currentY += wordSize.Y + WORD_SPACING;
            }
            
            _spriteBatch.DrawString(_font, word, new Vector2(currentX, currentY), _wordColors[i]);

            currentX += wordSize.X + WORD_SPACING;
        }
    }

    private void DisplayWordFrequency()
    {
        if (_freqData == null || _freqData.Count == 0)
        {
            _spriteBatch.DrawString(_font, "wordfrewuency.txt not loaded", new Vector2(WORD_SPACING, WORD_SPACING), Color.Black);
            return;
        }
        int maxFreq = 1;
        int maxCount = 1;

        for (int i = 0; i < _freqData.Count; i++)
        {
            if (_freqData[i].freq > maxFreq) maxFreq = _freqData[i].freq;
            if (_freqData[i].count > maxCount) maxCount = _freqData[i].count;
        }

        float left = WORD_SPACING + 40;
        float top = WORD_SPACING + 20;
        float right = CANVAS_WIDTH - WORD_SPACING;
        float bottom = CANVAS_HEIGHT - WORD_SPACING - 30;

        float plotW = right - left;
        float plotH = bottom - top;

        _spriteBatch.Draw(_pixel, new Rectangle((int)left, (int)bottom, (int)plotW, 2), Color.Black);
        _spriteBatch.Draw(_pixel, new Rectangle((int)left, (int)top, 2, (int)plotH), Color.Black);

        _spriteBatch.DrawString(_font, "Word Frequency", new Vector2(WORD_SPACING, WORD_SPACING), Color.Black);

        int barThickness = 6;

        for (int i = 0; i < _freqData.Count; i++)
        {
            int freq = _freqData[i].freq;
            int count = _freqData[i].count;

            float yRatio;

            if (maxFreq == 1)
            {
                yRatio = 0f;
            }
            else
            {
                yRatio = (float)(freq - 1)/(maxFreq - 1);
            }
            float y = bottom - yRatio * plotH;

            float xRatio = (float)count / maxCount;
            float w = xRatio * plotW;

            _spriteBatch.Draw(
            _pixel,
            new Rectangle((int)left, (int)(y - barThickness / 2), (int)w, barThickness),
            _deepBlue
        );

            if (freq == 1 || freq == 2 || freq == 5 || freq == 10 || freq == 20 || i == _freqData.Count - 1)
            {
                _spriteBatch.DrawString(_font, freq.ToString(), new Vector2(WORD_SPACING, y - 10), Color.Black);
            }   
        }
        _spriteBatch.DrawString(_font,"# of words with frequency", new Vector2(left, bottom + 8), Color.Black);
    }
}