using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public enum Direction {
    North,
    West,
    East,
    South,
    NorthWest,
    NorthEast,
    SouthWest,
    SouthEast
}

public static class WorldDataHelper {
    // public static readonly Vector2Int UpLeft = new Vector2Int(-1, 1);
    // public static readonly Vector2Int Up = new Vector2Int(0, 1);
    // public static readonly Vector2Int UpRight = new Vector2Int(1, 1);
    // public static readonly Vector2Int Left = new Vector2Int(-1, 0);
    // public static readonly Vector2Int Zero = new Vector2Int(0, 0);
    // public static readonly Vector2Int Right = new Vector2Int(1, 0);
    // public static readonly Vector2Int DownRight = new Vector2Int(1, -1);
    // public static readonly Vector2Int Down = new Vector2Int(0, -1);
    // public static readonly Vector2Int DownLeft = new Vector2Int(-1, -1);
    // public static readonly Vector2Int One = new Vector2Int(1, 1);

    public static Vector2 N(this Vector2 v) => new Vector2(0, 1);
    public static Vector2 W(this Vector2 v) => new Vector2(-1, 0);
    public static Vector2 E(this Vector2 v) => new Vector2(1, 0);
    public static Vector2 S(this Vector2 v) => new Vector2(0, -1);
    public static Vector2 NW(this Vector2 v) => new Vector2(-1, 1);
    public static Vector2 NE(this Vector2 v) => new Vector2(1, 1);
    public static Vector2 SW(this Vector2 v) => new Vector2(-1, -1);
    public static Vector2 SE(this Vector2 v) => new Vector2(1, -1);
    public static Vector2 One(this Vector2 v) => new Vector2(1, 1);
    public static Vector2 Zero(this Vector2 v) => new Vector2(0, 0);

    public static Vector2Int N(this Vector2Int v) => new Vector2Int(0, 1);
    public static Vector2Int W(this Vector2Int v) => new Vector2Int(-1, 0);
    public static Vector2Int E(this Vector2Int v) => new Vector2Int(1, 0);
    public static Vector2Int S(this Vector2Int v) => new Vector2Int(0, -1);
    public static Vector2Int NW(this Vector2Int v) => new Vector2Int(-1, 1);
    public static Vector2Int NE(this Vector2Int v) => new Vector2Int(1, 1);
    public static Vector2Int SW(this Vector2Int v) => new Vector2Int(-1, -1);
    public static Vector2Int SE(this Vector2Int v) => new Vector2Int(1, -1);
    public static Vector2Int One(this Vector2Int v) => new Vector2Int(1, 1);
    public static Vector2Int Zero(this Vector2Int v) => new Vector2Int(0, 0);

    public static Vector3 N(this Vector3 v) => new Vector3(0, 1, 0);
    public static Vector3 W(this Vector3 v) => new Vector3(-1, 0, 0);
    public static Vector3 E(this Vector3 v) => new Vector3(1, 0, 0);
    public static Vector3 S(this Vector3 v) => new Vector3(0, -1, 0);
    public static Vector3 NW(this Vector3 v) => new Vector3(-1, 1, 0);
    public static Vector3 NE(this Vector3 v) => new Vector3(1, 1, 0);
    public static Vector3 SW(this Vector3 v) => new Vector3(-1, -1, 0);
    public static Vector3 SE(this Vector3 v) => new Vector3(1, -1, 0);
    public static Vector3 One(this Vector3 v) => new Vector3(1, 1, 1);
    public static Vector3 Zero(this Vector3 v) => new Vector3(0, 0, 0);

    public static Vector3Int N(this Vector3Int v) => new Vector3Int(0, 1, 0);
    public static Vector3Int W(this Vector3Int v) => new Vector3Int(-1, 0, 0);
    public static Vector3Int E(this Vector3Int v) => new Vector3Int(1, 0, 0);
    public static Vector3Int S(this Vector3Int v) => new Vector3Int(0, -1, 0);
    public static Vector3Int NW(this Vector3Int v) => new Vector3Int(-1, 1, 0);
    public static Vector3Int NE(this Vector3Int v) => new Vector3Int(1, 1, 0);
    public static Vector3Int SW(this Vector3Int v) => new Vector3Int(-1, -1, 0);
    public static Vector3Int SE(this Vector3Int v) => new Vector3Int(1, -1, 0);
    public static Vector3Int One(this Vector3Int v) => new Vector3Int(1, 1, 1);
    public static Vector3Int Zero(this Vector3Int v) => new Vector3Int(0, 0, 0);

    public static Vector3Int ChunkPositionFromTileCoords(WorldGenerator world, Vector3Int pos) {
        return new Vector3Int {
            x = Mathf.FloorToInt(pos.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(pos.y / (float)world.chunkSize) * world.chunkSize,
            z = 0,
        };
    }

    public static Vector3Int ChunkPositionFromTileCoords(int chunkSize, Vector3Int pos) {
        return new Vector3Int {
            x = Mathf.FloorToInt(pos.x / (float)chunkSize) * chunkSize,
            y = Mathf.FloorToInt(pos.y / (float)chunkSize) * chunkSize,
            z = 0,
        };
    }

    public static Vector3Int ChunkIDFromChunkPosition(int chunkSize, Vector3Int pos) {
        return new Vector3Int {
            x = pos.x / chunkSize,
            y = pos.y / chunkSize,
            z = 0,
        };
    }

    public static Vector3Int ChunkIDFromTileCoords(int chunkSize, Vector3Int coords) {
        return ChunkPositionFromTileCoords(chunkSize, coords);
    }

    public static List<Vector3Int> GetNearPositions(Vector3Int pos, int distance) {
        List<Vector3Int> positions = new List<Vector3Int>();

        for (int y = -distance; y <= distance; y++) {
            for (int x = -distance; x <= distance; x++) {
                positions.Add(pos + new Vector3Int(x, y, 0));
            }
        }

        return positions;
    }

    public static List<Vector3Int> GetNearPositions(Vector3Int pos, int distance, Direction direction) {
        List<Vector3Int> positions = new List<Vector3Int>();

        if (direction == Direction.North || direction == Direction.NorthEast) {
            for (int y = distance; y >= -distance; y--) {
                for (int x = -distance; x <= distance; x++) {
                    positions.Add(pos + new Vector3Int(x, y, 0));
                }
            }
        }
        else if (direction == Direction.West || direction == Direction.NorthWest) {    
            for (int x = -distance; x <= distance; x++) {
                for (int y = -distance; y <= distance; y++) {
                    positions.Add(pos + new Vector3Int(x, y, 0));
                }
            }
        }
        else if (direction == Direction.East || direction == Direction.SouthEast) {
            for (int x = distance; x >= distance; x--) {
                for (int y = distance; y >= -distance; y--) {
                    positions.Add(pos + new Vector3Int(x, y, 0));
                }
            }
        }
        else if (direction == Direction.South || direction == Direction.SouthWest) {
            for (int y = -distance; y <= distance; y++) {
                for (int x = distance; x >= -distance; x--) {
                    positions.Add(pos + new Vector3Int(x, y, 0));
                }
            }
        }

        return positions;
    }

    public static Direction GetDirection(Vector3Int vDir) {
        if (vDir == Vector3Int.up) {
            return Direction.North;
        }
        else if (vDir == Vector3Int.left) {
            return Direction.West;
        }
        else if (vDir == Vector3Int.right) {
            return Direction.East;
        }
        else if (vDir == Vector3Int.down) {
            return Direction.South;
        }
        else if (vDir == Vector3Int.up + Vector3Int.left) {
            return Direction.NorthWest;
        }
        else if (vDir == Vector3Int.up + Vector3Int.right) {
            return Direction.NorthEast;
        }
        else if (vDir == Vector3Int.down + Vector3Int.left) {
            return Direction.SouthWest;
        }
        else if (vDir == Vector3Int.down + Vector3Int.right) {
            return Direction.SouthEast;
        }

        return Direction.North;
    }

    public static Direction GetOppositeDirection(Direction dir) {
        Direction opp;

        switch (dir) {
            case Direction.North:
                opp = Direction.South;
                break;
            case Direction.West:
                opp = Direction.East;
                break;
            case Direction.East:
                opp = Direction.West;
                break;
            case Direction.South:
                opp = Direction.North;
                break;
            case Direction.NorthWest:
                opp = Direction.SouthEast;
                break;
            case Direction.NorthEast:
                opp = Direction.SouthWest;
                break;
            case Direction.SouthWest:
                opp = Direction.NorthEast;
                break;
            case Direction.SouthEast:
                opp = Direction.NorthWest;
                break;
            default:
                opp = Direction.North;
                break;
        }

        return opp;
    }

    public static int ManhattanDistance(Vector3Int a, Vector3Int b) {
        checked {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b) {
        checked {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    // public static bool IsInsideBounds(int x, int y) {

    // }
}