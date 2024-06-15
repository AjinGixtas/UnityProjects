using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GenerateTerrain : MonoBehaviour {
    [SerializeField] private Material plainMaterial, oceanMaterial, swampMaterial, mountainMaterial, forestMaterial, beachMaterial, snowMaterial, waterMaterial;
    public GameObject tilePrefab;
    public Transform tileContainer;
    public int column, row;
    public int collapsedTile = 0;
    public int[] intialTileState;

    private static Dictionary<int, int[]> adjacencyRules = new Dictionary<int, int[]>() {
        {PLAIN, new int[] {FOREST, MOUNTAIN, BEACH}},
        {OCEAN, new int[] {WATER, BEACH}},
        {SWAMP, new int[] {FOREST, WATER}},
        {MOUNTAIN, new int[] {PLAIN, FOREST, SNOW}},
        {FOREST, new int[] {PLAIN, MOUNTAIN, SWAMP}},
        {BEACH, new int[] {PLAIN, OCEAN, WATER}},
        {SNOW, new int[] {MOUNTAIN, WATER}},
        {WATER, new int[] {OCEAN, BEACH, SWAMP, SNOW}}
    };

    public const int PLAIN = 0, OCEAN = 1, SWAMP = 2, MOUNTAIN = 3, FOREST = 4, BEACH = 5, SNOW = 6, WATER = 7;
    int[] EDGE_ADJACENT_OFFSET = new int[4];
    int[] CORNER_ADJACENT_OFFSET = new int[4];
    int[] ADJACENT_OFFSET = new int[8];

    Tile[] boardObjectArray;
    GameObject[] boardGameObjectArray;

    void IntializeOffsetLookUpTable() {
        EDGE_ADJACENT_OFFSET = new int[] { -column, -1, 1, column };
        CORNER_ADJACENT_OFFSET = new int[] { -column - 1, -column + 1, column - 1, column + 1 };
        ADJACENT_OFFSET = new int[] { -column, -1, 1, column, -column - 1, -column + 1, column - 1, column + 1 };
    }
    void IntializeVariable() {
        boardObjectArray = new Tile[column * row];
        boardGameObjectArray = new GameObject[column * row];
    }
    void IntializeBoardObjectArray() {
        for (int x = 0; x < column; x++) {
            for (int y = 0; y < row; y++) {
                int index = GetIndexBasedOnCoordinate(x, y);
                boardObjectArray[index] = new Tile(x, y, this, boardGameObjectArray[index]);
            }
        }
    }
    void IntializeBoardGameObjectArray() {
        for (int x = 0; x < column; x++) {
            for (int y = 0; y < row; y++) {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x * 10, 0, y * 10), new Quaternion(0, 0, 0, 0), tileContainer);

                tile.name = $"X_{x}_Y_{y}";

                int index = GetIndexBasedOnCoordinate(x, y);
                boardGameObjectArray[index] = tile;
            }
        }
    }
    void WaveFunctionCollapse() {
        for (int i = 0; i < intialTileState.Length; i++) {
            while (true) {
                int xPos = Random.Range(0, column - 1), yPos = Random.Range(0, row - 1);
                int index = GetIndexBasedOnCoordinate(xPos, yPos);
                bool isSuccess = boardObjectArray[index].ReduceEntropyToOne(intialTileState[i]);
                ReduceEntropyOfNeighbourTile(xPos, yPos);
                if (isSuccess) { break; }
            }
        }
        // Keep doing stuff until the whole board has collapsed
        while (collapsedTile != row * column) {
            int lowestAmountOfEntropy = int.MaxValue;
            Tile[] tileWithLowestEntropy = new Tile[0];
            for (int i = 0; i < boardObjectArray.Length; i++) {

                int amountOfEntropy = boardObjectArray[i].GetAmountOfEntropy();

                if (amountOfEntropy > lowestAmountOfEntropy || amountOfEntropy == 1) { continue; }

                if (lowestAmountOfEntropy == amountOfEntropy) {
                    AppendElementToArray(ref tileWithLowestEntropy, ref boardObjectArray[i]);
                    continue;
                }

                lowestAmountOfEntropy = amountOfEntropy;
                tileWithLowestEntropy = new Tile[] { boardObjectArray[i] };
            }

            Tile chosenTile = tileWithLowestEntropy[Random.Range(0, tileWithLowestEntropy.Length - 1)];
            chosenTile.ReduceEntropyToOne();
            (int xPos, int yPos) = (chosenTile.GetCoordinate()[0], chosenTile.GetCoordinate()[1]);
            ReduceEntropyOfNeighbourTile(xPos, yPos);
        }
    }

    void Start() {
        IntializeVariable();
        IntializeBoardGameObjectArray();
        IntializeBoardObjectArray();
        IntializeOffsetLookUpTable();
        WaveFunctionCollapse();
        LogResult();
    }

    private void ReduceEntropyOfNeighbourTile(int xPos, int yPos) {
        int tileIndex = GetIndexBasedOnCoordinate(xPos, yPos);
        int terrainType = boardObjectArray[tileIndex].GetCollapsedState();

        /* This snippet of code is intend for general-purpose wave function collapse algorithm or large scale apllication
        int[] allOffset = GetOffsetOfTerrain(terrainType);
        */
        int[] allOffset = new int[] { -column - 1, -column, -column + 1, -1, +1, column - 1, column, column + 1 };

        for (int i = 0; i < allOffset.Length; i++) {

            int neighbourIndex = tileIndex + allOffset[i];

            if (neighbourIndex < 0 || neighbourIndex >= row * column) { continue; }
            if (boardObjectArray[neighbourIndex].GetAmountOfEntropy() == 1) { continue; }
            int[] possibleStateOfNeighbour = GetPossibleNeighbourState(terrainType);
            boardObjectArray[neighbourIndex].ReduceEntropy(possibleStateOfNeighbour);
        }
    }
    private int GetIndexBasedOnCoordinate(int xPos, int yPos) {
        return xPos * column + yPos;
    }
    private static int[] GetPossibleNeighbourState(int terrainIndex) {
        return adjacencyRules[terrainIndex];
    }
    private void LogResult() {
        string result = "";
        for (int i = 0; i < column; i++) {
            string rowString = "";
            for (int j = 0; j < row; j++) {
                int index = GetIndexBasedOnCoordinate(i, j);
                int value = boardObjectArray[index].GetCollapsedState(); // Call your function here
                rowString += value.ToString() + " ";
            }
            result = result + rowString + "\n";
        }
        Debug.Log(result);
    }
    /* This snippet of code is intend for general-purpose wave function collapse algorithm or large scale apllication
    private static int[] GetPossibleRemainingTerrain(int terrainIndex, int offsetIndex) {
        
        return terrainIndex switch {
            PLAIN => plainDict.ElementAt(offsetIndex).value,
            ISLAND => islandDict.ElementAt(offsetIndex).value,
            SWAMP => swampDict.ElementAt(offsetIndex).value,
            MOUNTAIN => mountainDict.ElementAt(offsetIndex).value,
            FOREST => forestDict.ElementAt(offsetIndex).value,
            _ => throw new Exception($"Invalid terrain index! Terrain index given: {terrainIndex}"),
        };
        
    }
    */

    // Might introduce more overload later on
    private void AppendElementToArray(ref Tile[] arr, ref Tile element) {
        Array.Resize(ref arr, arr.Length + 1);
        arr[^1] = element;
    }
    /* These 2 function is accessed exclusively for Tile.cs only, if these function is used else where, state it right here:
    */
    public void IncrementCollapsedTileCount() {
        collapsedTile++;
    }
    public void RenderTile(GameObject tile, int collapsedState) {
        tile.GetComponent<Renderer>().material = collapsedState switch {
            PLAIN => plainMaterial,
            OCEAN => oceanMaterial,
            SWAMP => swampMaterial,
            MOUNTAIN => mountainMaterial,
            FOREST => forestMaterial,
            BEACH => beachMaterial,
            SNOW => snowMaterial,
            WATER => waterMaterial,
            _ => throw new Exception($"State index out of range! Index given: {collapsedState}")
        };
    }
}
