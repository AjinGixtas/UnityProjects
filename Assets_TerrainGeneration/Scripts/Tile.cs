using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile {
    public const int PLAIN = 0;
    public const int OCEAN = 1;
    public const int SWAMP = 2;
    public const int MOUNTAIN = 3;
    public const int FOREST = 4;
    public const int BEACH = 5;
    public const int SNOW = 6;
    public const int WATER = 7;
    readonly static int[] SUPER_POSITION = new int[] { PLAIN, OCEAN, SWAMP, MOUNTAIN, FOREST, BEACH, SNOW, WATER };

    private int[] possibleState = SUPER_POSITION;
    private bool hasCollapsed = false;

    private int collapsedState;
    private int xPos, yPos;
    private GameObject tile;

    private GenerateTerrain terrainManager;
    public Tile(int xPos, int yPos, GenerateTerrain terrainManager, GameObject tile) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.tile = tile;
        this.terrainManager = terrainManager;
    }
    public void ReduceEntropy(int[] possibleStateRemaining) {
        if(hasCollapsed) { return; }
        possibleState = possibleStateRemaining.Intersect(possibleState).ToArray();
        TryCollapse();
    }
    // This function is overloaded, one is for explicit collapse, the other is implicit collapse
    public bool ReduceEntropyToOne(int stateIndex) {
        if (hasCollapsed) { return false; }

        if (Array.IndexOf(possibleState, stateIndex) == -1) {
            return false;
            // throw new Exception($"The tile X_{xPos}_Y_{yPos} is impossible to collapsed into the given state! State index given: {stateIndex}");
        }
        possibleState = new int[] { stateIndex };
        TryCollapse();
        return true;
    }
    public bool ReduceEntropyToOne() {
        if (hasCollapsed) { return false; }
        possibleState = new int[] { possibleState[Random.Range(0, possibleState.Length - 1)] };
        TryCollapse();
        return true;
    }

    private void TryCollapse() {
        if(possibleState.Length == 0) {
            throw new Exception($"The tile X_{xPos}_Y_{yPos} has no possible state to collapse!");
        }
        if (possibleState.Length != 1) {
            return;
        }
        collapsedState = possibleState[0];
        hasCollapsed = true;

        terrainManager.RenderTile(tile, collapsedState);
        terrainManager.IncrementCollapsedTileCount();


    }
    public int[] GetCoordinate() {
        return new int[] { xPos, yPos };
    }
    public int GetCollapsedState() {
        if(!hasCollapsed) { throw new Exception("Tile has not been collapsed yet!"); }
        return collapsedState;
    }
    public int GetAmountOfEntropy() {
        return possibleState.Length;
    }
}
