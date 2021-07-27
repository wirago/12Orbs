using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbs.Interface.TextInput
{
    public class TextBox
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public Rectangle Area
        {
            get { return Renderer.Area; }
            set { Renderer.Area = value; }
        }

        public readonly Text Text;
        public readonly TextRenderer Renderer;
        public readonly Cursor Cursor;
        private string question = "";

        public event EventHandler<KeyboardInput.KeyEventArgs> EnterDown;

        private string clipboard;

        public bool IsActive { get; set; }

        int commandHistorytracker = 0;

        public TextBox(Rectangle area, int maxCharacters, string text, GraphicsDevice graphicsDevice,
            SpriteFont spriteFont, Color cursorColor, Color selectionColor, int ticksPerToggle)
        {
            GraphicsDevice = graphicsDevice;

            Text = new Text(maxCharacters)
            {
                String = text
            };

            Renderer = new TextRenderer(this)
            {
                Area = area,
                Font = spriteFont,
                Color = Color.Black
                
            };

            Cursor = new Cursor(this, cursorColor, selectionColor, new Rectangle(0, 0, 1, 1), ticksPerToggle);

            KeyboardInput.CharPressed += CharacterTyped;
            KeyboardInput.KeyPressed += KeyPressed;
        }

        public TextBox(Rectangle area, int maxCharacters, string text, GraphicsDevice graphicsDevice,
            SpriteFont spriteFont, Color cursorColor, Color selectionColor, int ticksPerToggle, string question)
        {
            GraphicsDevice = graphicsDevice;
            this.question = question;

            UIManager.ShowQuestion(question, new Vector2(area.X, area.Y - 120));

            Text = new Text(maxCharacters)
            {
                String = text
            };

            Renderer = new TextRenderer(this)
            {
                Area = area,
                Font = spriteFont,
                Color = Color.Black

            };

            Cursor = new Cursor(this, cursorColor, selectionColor, new Rectangle(0, 0, 1, 1), ticksPerToggle);

            KeyboardInput.CharPressed += CharacterTyped;
            KeyboardInput.KeyPressed += KeyPressed;
        }

        public void Dispose()
        {
            KeyboardInput.Dispose();
        }

        public void Clear()
        {
            Text.RemoveCharacters(0, Text.Length);
            Cursor.TextCursor = 0;
            Cursor.SelectedChar = null;
            UIManager.textInputOpen = false;
        }

        private void KeyPressed(object sender, KeyboardInput.KeyEventArgs e, KeyboardState ks)
        {
            if (IsActive)
            {
                int oldPos = Cursor.TextCursor;
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                    {
                            //EnterDown?.Invoke(this, e);
                            commandHistorytracker = 0;
                            if(question == "")
                                InputHandler.HandleTextInput(this.Text.String, true);
                            else
                            {
                                UIManager.ClearNotifications();
                                UIManager.textInputOpen = false;
                                InputHandler.HandleTextInput(this.Text.String, false);
                            }

                            Clear();
                            IsActive = false;                            
                            
                        break;
                    }
                    case Keys.Left:
                        if (KeyboardInput.CtrlDown)
                            Cursor.TextCursor = IndexOfLastCharBeforeWhitespace(Cursor.TextCursor, Text.Characters);
                        else
                            Cursor.TextCursor--;

                        ShiftMod(oldPos);
                        break;
                    case Keys.Right:
                        if (KeyboardInput.CtrlDown)
                            Cursor.TextCursor = IndexOfNextCharAfterWhitespace(Cursor.TextCursor, Text.Characters);
                        else
                            Cursor.TextCursor++;

                        ShiftMod(oldPos);
                        break;
                    case Keys.Up:
                        if (Commander.commandHistory.Count != 0)
                        {
                            if (commandHistorytracker < Commander.commandHistory.Count)
                            {
                                Text.String = Commander.commandHistory[commandHistorytracker];
                                Cursor.TextCursor = Text.Length;
                                if (commandHistorytracker < Commander.commandHistory.Count - 1)
                                    commandHistorytracker++;
                            }
                        }
                        //Text.String += commandHistorytracker;
                        break;
                    case Keys.Down:
                        if (Commander.commandHistory.Count != 0)
                        {
                            if (commandHistorytracker > 0)
                            {
                                Text.String = Commander.commandHistory[commandHistorytracker-1];
                                Cursor.TextCursor = Text.Length;
                                    commandHistorytracker--;
                            }
                            else
                            {
                                Text.String = "";
                                Cursor.TextCursor = Text.Length;
                            }
                        }
                        //Text.String += commandHistorytracker;
                        break;
                    case Keys.Home:
                        Cursor.TextCursor = 0;
                        ShiftMod(oldPos);
                        break;
                    case Keys.End:
                        Cursor.TextCursor = Text.Length;
                        ShiftMod(oldPos);
                        break;
                    case Keys.Delete:
                        if (DelSelection() == null && Cursor.TextCursor < Text.Length)
                            Text.RemoveCharacters(Cursor.TextCursor, Cursor.TextCursor + 1);

                        break;
                    case Keys.Back:
                        if (DelSelection() == null && Cursor.TextCursor > 0)
                        {
                            Text.RemoveCharacters(Cursor.TextCursor - 1, Cursor.TextCursor);
                            Cursor.TextCursor--;
                        }
                        break;
                    case Keys.A:
                        if (KeyboardInput.CtrlDown)
                            if (Text.Length > 0)
                            {
                                Cursor.SelectedChar = 0;
                                Cursor.TextCursor = Text.Length;
                            }
                        break;
                    case Keys.C:
                        if (KeyboardInput.CtrlDown)
                            clipboard = DelSelection(true);

                        break;
                    case Keys.X:
                        if (KeyboardInput.CtrlDown)
                            if (Cursor.SelectedChar.HasValue)
                                clipboard = DelSelection();

                        break;
                    case Keys.V:
                        if (KeyboardInput.CtrlDown)
                            if (clipboard != null)
                            {
                                DelSelection();
                                foreach (char c in clipboard)
                                {
                                    if (Text.Length < Text.MaxLength)
                                    {
                                        Text.InsertCharacter(Cursor.TextCursor, c);
                                        Cursor.TextCursor++;
                                    }
                                }
                            }
                        break;
                }
            }
        }

        private void ShiftMod(int oldPos)
        {
            if (KeyboardInput.ShiftDown)
            {
                if (Cursor.SelectedChar == null)
                    Cursor.SelectedChar = oldPos;
            }
            else
            {
                Cursor.SelectedChar = null;
            }
        }

        private void CharacterTyped(object sender, KeyboardInput.CharacterEventArgs e, KeyboardState ks)
        {
            if (IsActive && !KeyboardInput.CtrlDown)
            {
                if (IsLegalCharacter(Renderer.Font, e.Character) && !e.Character.Equals('\r') &&
                    !e.Character.Equals('\n'))
                {
                    DelSelection();
                    if (Text.Length < Text.MaxLength)
                    {
                        Text.InsertCharacter(Cursor.TextCursor, e.Character);
                        Cursor.TextCursor++;
                    }
                }
            }
        }

        private string DelSelection(bool fakeForCopy = false)
        {
            if (!Cursor.SelectedChar.HasValue)
                return null;

            int tc = Cursor.TextCursor;
            int sc = Cursor.SelectedChar.Value;
            int min = Math.Min(sc, tc);
            int max = Math.Max(sc, tc);
            string result = Text.String.Substring(min, max - min);

            if (!fakeForCopy)
            {
                Text.Replace(Math.Min(sc, tc), Math.Max(sc, tc), string.Empty);
                if (Cursor.SelectedChar.Value < Cursor.TextCursor)
                {
                    Cursor.TextCursor -= tc - sc;
                }
                Cursor.SelectedChar = null;
            }
            return result;
        }

        public static bool IsLegalCharacter(SpriteFont font, char c)
        {
            return font.Characters.Contains(c) || c == '\r' || c == '\n';
        }

        public static int IndexOfNextCharAfterWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;
            char c = chars[pos];
            bool whiteSpaceFound = false;
            while (true)
            {
                if (c.Equals(' '))
                    whiteSpaceFound = true;
                else if (whiteSpaceFound)
                    return pos;

                ++pos;

                if (pos >= chars.Length)
                    return chars.Length;

                c = chars[pos];
            }
        }

        public static int IndexOfLastCharBeforeWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;

            bool charFound = false;
            while (true)
            {
                --pos;
                if (pos <= 0)
                    return 0;

                var c = chars[pos];

                if (c.Equals(' '))
                {
                    if (charFound)
                        return ++pos;
                }
                else
                {
                    charFound = true;
                }
            }
        }

        public void Update()
        {
            Renderer.Update();
            Cursor.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Renderer.Draw(spriteBatch);

            if (IsActive)
                Cursor.Draw(spriteBatch);
        }
    }
}
