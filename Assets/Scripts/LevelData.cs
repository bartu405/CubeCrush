using System;

[Serializable]
public class LevelData
{

    // Buraya daha fazla data ekleyebilirsin, mesela obstacle count, kaç tür obstacle var
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid; // This will store the grid configuration from your JSON
}
