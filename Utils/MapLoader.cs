using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Entities;
using Arcade2D.Managers;

namespace Arcade2D.Utils;

public class MapLoader
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly EntityManager _entityManager;
    private readonly Vector2 _mapOffset = new Vector2(16, 16);
    private const int TileSize = 32;

    public MapLoader(GraphicsDevice graphicsDevice, EntityManager entityManager)
    {
        _graphicsDevice = graphicsDevice;
        _entityManager = entityManager;
    }

    public Texture2D LoadSpriteSheetWithChromaKey(string path, Color chromaKey)
    {
        if (!File.Exists(path)) return null;

        using var stream = File.OpenRead(path);
        var texture = Texture2D.FromStream(_graphicsDevice, stream);

        Color[] colorData = new Color[texture.Width * texture.Height];
        texture.GetData(colorData);
        for (int i = 0; i < colorData.Length; i++)
        {
            if (colorData[i].R == chromaKey.R && colorData[i].G == chromaKey.G && colorData[i].B == chromaKey.B)
            {
                colorData[i] = Color.Transparent;
            }
        }
        texture.SetData(colorData);
        return texture;
    }

    public Player BuildMap(string filePath, Texture2D spriteSheet)
    {
        Texture2D wallTexture = new Texture2D(_graphicsDevice, 1, 1);
        wallTexture.SetData(new[] { ColorPalette.Lavender });

        Texture2D pelletTexture = new Texture2D(_graphicsDevice, 1, 1);
        pelletTexture.SetData(new[] { ColorPalette.SoftYellow });

        // ДОДАНО: Окрема рожева текстура для бонусів швидкості
        Texture2D speedPelletTexture = new Texture2D(_graphicsDevice, 1, 1);
        speedPelletTexture.SetData(new[] { Color.HotPink });

        // Створюємо та додаємо гравця
        Player player = new Player(new Vector2(32 * 10 + 4, 32 * 10 + 4) + _mapOffset, spriteSheet);
        _entityManager.Add(player);

        // Додаємо 4 класичних привидів з їх унікальними індексами кольорів
        _entityManager.Add(new Ghost(new Vector2(32 * 1, 32 * 1) + _mapOffset, spriteSheet, 0));
        _entityManager.Add(new Ghost(new Vector2(32 * 19, 32 * 1) + _mapOffset, spriteSheet, 1));
        _entityManager.Add(new Ghost(new Vector2(32 * 1, 32 * 19) + _mapOffset, spriteSheet, 2));
        _entityManager.Add(new Ghost(new Vector2(32 * 19, 32 * 19) + _mapOffset, spriteSheet, 3));

        List<string> mapLines = new List<string>();
        if (File.Exists(filePath))
        {
            mapLines = File.ReadAllLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        }
        else
        {
            mapLines = new List<string>
            {
                "111111111111111111111",
                "100000000000000000001",
                "111111111111111111111"
            };
        }

        for (int row = 0; row < mapLines.Count; row++)
        {
            string currentLine = mapLines[row];
            for (int col = 0; col < currentLine.Length; col++)
            {
                Vector2 pos = new Vector2(col * TileSize, row * TileSize) + _mapOffset;
                char tileChar = currentLine[col];

                if (tileChar == '1')
                {
                    _entityManager.Add(new Wall(pos, wallTexture));
                }
                else if (tileChar == '0')
                {
                    bool isPowerPellet = (row == 1 && col == 1) || (row == 1 && col == 19) || (row == 19 && col == 1) || (row == 19 && col == 19);
                    bool isSpeedPellet = (row == 9 && col == 1) || (row == 9 && col == 19);

                    if (isPowerPellet)
                    {
                        _entityManager.Add(new PowerPellet(pos + new Vector2(8, 8), pelletTexture));
                    }
                    else if (isSpeedPellet)
                    {
                        // ВИПРАВЛЕНО: Використовуємо speedPelletTexture
                        _entityManager.Add(new SpeedPellet(pos + new Vector2(8, 8), speedPelletTexture));
                    }
                    else if ((row + col) % 2 == 0)
                    {
                        if (!((row >= 9 && row <= 11) && (col >= 9 && col <= 11)))
                        {
                            _entityManager.Add(new Pellet(pos + new Vector2(10, 10), pelletTexture));
                        }
                    }
                }
            }
        }

        return player;
    }
}