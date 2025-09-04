using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NOTE This should be a singleton or all static methods class...
public class BlocksController : MonoBehaviour
{
    public Transform PlayerObjectTransform;
    public Transform ParentObject;
    public Transform TerrainsParent;
    private GameObject[,] _Blocks;
    /// <summary>
    /// The Largest z value for our terrains
    /// </summary>
    private float _MaxHeight;
    /// <summary>
    /// The Largest x value for our terrains
    /// </summary>
    private float _MaxWidth;
    private float _MinWidth;
    private float _MinHeight;

    public int BlocksPerRow;
    public int BlocksPerCol;
    /// <summary>
    /// The x range for one block
    /// </summary>
    private float _WidthPerBlock;
    /// <summary>
    /// The z range for one block
    /// </summary>
    private float _HeightPerBlock;
    /// <summary>
    /// The row index of the block player current at
    /// </summary>
    private int _CurrentRow;
    /// <summary>
    /// The col index of the block player current at
    /// </summary>
    private int _CurrentCol;

    private Vector3 _Scale;
    // Start is called before the first frame update
    void Awake()
    {
        _Scale = new Vector3(1.25f, 1.25f, 1.25f);
        _Blocks = new GameObject[BlocksPerRow,BlocksPerCol];
        _MaxWidth = _MaxHeight = _MinWidth = _MinHeight = 0.0f;
        foreach (Transform childTransform  in TerrainsParent) {
            TerrainData terrainData = childTransform.GetComponent<Terrain>().terrainData;
            if (childTransform.position.x > _MaxWidth) _MaxWidth = childTransform.position.x + terrainData.size.x;
            if (childTransform.position.z > _MaxHeight) _MaxHeight = childTransform.position.z + terrainData.size.z;
            if (childTransform.position.z < _MinHeight) _MinHeight = childTransform.position.z;
            if (childTransform.position.x < _MinWidth) _MinWidth = childTransform.position.x;
            Debug.Log(childTransform.position.x);
        }
        //BUG This works only if all terrains x and z(worldCoord) STARTS at zero and GROW POSITIVELY
        _WidthPerBlock = (_MaxWidth -_MinWidth) / BlocksPerRow;
        _HeightPerBlock = (_MaxHeight - _MinHeight) / BlocksPerCol;
        for (int i = 0; i < BlocksPerRow; i++) {
            for (int j = 0; j < BlocksPerCol; j++) {
                // Create a new GameObject instance
                GameObject block = new GameObject("Block_" + i + "_" + j);
                block.transform.parent = ParentObject.transform;
                block.transform.position = new Vector3(_MinWidth + _WidthPerBlock * i, 0, _MinHeight +_HeightPerBlock * j);
                block.SetActive(false);
                _Blocks[i, j] = block;
            }
        }
        
        StartCoroutine(EnableBlocks());
    }

    public void PlaceAtRightParentObject(GameObject obj, Vector3 worldCoord, Quaternion q)
    {
        int row = BinarySearchRow(worldCoord.x);
        if(row == -1) return;
        int col = BinarySearchCol(worldCoord.z);
        if (col == -1) return;
        GameObject newObj = Instantiate(obj, worldCoord, q, _Blocks[row,col].transform);
        newObj.transform.localScale = _Scale;
        newObj.isStatic = true;
    }
    
    //  Binary search to find the first element(INDEX) greater than or equal to value
    int BinarySearchRow(float value) {
        int left = 0;
        int right = BlocksPerRow-1;
        while (left <= right) {
            int mid = left + (right - left) / 2;
            float lowerBound = _MinWidth + mid * _WidthPerBlock;
            float upperBound = _MinWidth + (mid + 1) * _WidthPerBlock;
            if (value >= lowerBound && value < upperBound) {
                return mid; // Value falls within the range, return the current index
            } else if (value < lowerBound) {
                right = mid - 1; // Value is to the left of the current range
            } else {
                left = mid + 1; // Value is to the right of the current range
            }
        }
        // If no element is greater than or equal to value, return -1
        return -1;
    }
    int BinarySearchCol(float value) {
        int left = 0;
        int right = BlocksPerCol-1;
        while (left <= right) {
            int mid = left + (right - left) / 2;
            float lowerBound = _MinHeight + mid * _HeightPerBlock;
            float upperBound = _MinHeight + (mid + 1) * _HeightPerBlock;
            if (value >= lowerBound && value < upperBound) {
                return mid; // Value falls within the range, return the current index
            } else if (value < lowerBound) {
                right = mid - 1; // Value is to the left of the current range
            } else {
                left = mid + 1; // Value is to the right of the current range
            }
        }
        // If no element is greater than or equal to value, return -1
        return -1;
    }
    
    
    
    private float interval = 2f; // Time interval in seconds
    private float elapsedTime = 0f; // Tracks the elapsed time
    // Update is called once per frame
    void Update()
    {
        // Increment the elapsed time
        elapsedTime += Time.deltaTime;

        // Check if the elapsed time has exceeded the interval
        if (elapsedTime >= interval)
        {
            // Start the coroutine
            //TODO START the Disable Coroutine, and if something get removed, start Enable Coroutine
            StartCoroutine(DisableBlocks());

            // Reset the elapsed time
            elapsedTime = 0f;
        }
    }
    
    /// <summary>
    /// The function enables THE BLOCK player at PLUS 8 blocks surround player
    /// B0  B1  B2              |      -UP cols-     |
    /// B3  B4  B5          LEFT rows    center   RIGHT rows
    /// B6  B7  B8              |      -DOWN cols-   |
    /// So the scene only have 9 blocks enabled and player always inside B4
    /// Or player reaches the boundary of the scene
    /// </summary>
    // TODO It's better make air walls for the edge blocks 
    IEnumerator EnableBlocks() {
        Vector3 playerCoord = PlayerObjectTransform.position;
        //BUG: if the player runs beyond the scene???
        int row = BinarySearchRow(playerCoord.x);
        if (row == -1) yield return null;
        int col = BinarySearchCol(playerCoord.z);
        if (col == -1) yield return null;
        _CurrentRow = row;
        _CurrentCol = col;
        int upperRowLimit = row + 1 > BlocksPerRow - 1 ? BlocksPerRow : row + 2;
        int upperColLimit = col + 1 > BlocksPerCol - 1 ? BlocksPerCol : col + 2;
        for (int i = row > 0?row - 1 : 0; i < upperRowLimit; i++) {
            for (int j = col > 0?col - 1 : 0; j < upperColLimit; j++) {
                _Blocks[i,j].SetActive(true);
                // Debug.Log();
            }
        }
        // Wait for the specified interval
        yield return null;
    }
    
    
    /// B0  B1  B2       -1,-1      -1,~       -1,+1       |      -UP cols-     |
    /// B3  B4  B5       ~,-1   curRow,curCol   ~,+1    LEFT rows   center   RIGHT rows
    /// B6  B7  B8       +1,-1      +1,~       +1,+1       |      -DOWN cols-   |
    IEnumerator DisableBlocks() {
        Vector3 playerCoord = PlayerObjectTransform.position;
        //BUG: if the player runs beyond the scene???
        int row = BinarySearchRow(playerCoord.x);
        if (row == -1) yield return null;
        int col = BinarySearchCol(playerCoord.z);
        if (col == -1) yield return null;
        //                                      
        if(row == _CurrentRow && col == _CurrentCol) yield return null;
        if (row == _CurrentRow && col == _CurrentCol - 1) {//Move West, disable RIGHT ROWs
            if (_CurrentCol!= BlocksPerCol-1) {//if player used to at RIGHTMOST edge, do nothing
                //                                    UPPER index  +1           Same Thing but simplified
                int upperRowLimit =  BlocksPerRow - (_CurrentRow-1+1)  > 3 ? 3 : BlocksPerRow - _CurrentRow;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol+1].SetActive(false);
                }
            }
        }else if (row == _CurrentRow - 1 && col == _CurrentCol) {//Move North, disable DOWN COLs
            if (_CurrentRow != BlocksPerRow-1) { //if player used to at Bottom edge, do nothing
                int upperColLimit = BlocksPerCol - ((_CurrentCol-1)+1) > 3 ? 3 :BlocksPerCol - _CurrentCol;
                for (int i =0; i < upperColLimit; i++) {
                    _Blocks[_CurrentRow+1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        
        //BUG THIS WHOLE SECTION WRITTEN BY CHAT AND NEED CONSIDERATIONs
        else if (row == _CurrentRow && col == _CurrentCol + 1) {// Move East, disable LEFT ROWs
            if (_CurrentCol != 0) { // If player used to be at LEFTMOST edge, do nothing
                int upperRowLimit =  BlocksPerRow - (_CurrentRow-1+1)  > 3 ? 3 : BlocksPerRow - _CurrentRow;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol-1].SetActive(false);
                }
            }
        }
        else if (row == _CurrentRow + 1 && col == _CurrentCol) {// Move South, disable UP COLs
            if (_CurrentRow != 0){ // If player used to be at Top edge, do nothing
                int upperColLimit = BlocksPerCol - ((_CurrentCol-1)+1) > 3 ? 3 :BlocksPerCol - _CurrentCol;
                for (int i = 0; i< upperColLimit; i++) {
                    // Debug.Log("i: " + i + "j: " + (_CurrentCol - 1));
                    _Blocks[_CurrentRow-1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        else if (row == _CurrentRow - 1 && col == _CurrentCol - 1) {// Move Northwest, disable RIGHT ROWs & DOWN COLs
            if (_CurrentRow != BlocksPerRow - 1 && _CurrentCol != BlocksPerCol - 1) // If player used to be at Bottom or Right edge, do nothing
            {
                int upperRowLimit = BlocksPerRow - _CurrentRow > 3 ? 3 : BlocksPerRow - _CurrentRow;
                int upperColLimit = BlocksPerCol - _CurrentCol > 3 ? 3 : BlocksPerCol - _CurrentCol;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol+1].SetActive(false);
                }
                for (int i = 0; i < upperColLimit; i++) {
                    _Blocks[_CurrentRow+1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        else if (row == _CurrentRow - 1 && col == _CurrentCol + 1) {// Move Northeast, disable LEFT ROWs & DOWN COLs
            if (_CurrentRow != BlocksPerRow - 1 && _CurrentCol != 0){ // If player used to be at Bottom or Left edge, do nothing
                int upperRowLimit = BlocksPerRow - _CurrentRow > 3 ? 3 : BlocksPerRow - _CurrentRow;
                int upperColLimit = BlocksPerCol - _CurrentCol > 3 ? 3 : BlocksPerCol - _CurrentCol;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol-1].SetActive(false);
                }
                for (int i = 0; i < upperColLimit; i++) {
                    _Blocks[_CurrentRow+1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        else if (row == _CurrentRow + 1 && col == _CurrentCol + 1) { // Move Southeast, disable LEFT ROWs & UP COLs
            if (_CurrentRow != 0 && _CurrentCol != 0){ // If player used to be at Top or Left edge, do nothing
                int upperRowLimit = BlocksPerRow - _CurrentRow > 3 ? 3 : BlocksPerRow - _CurrentRow;
                int upperColLimit = BlocksPerCol - _CurrentCol > 3 ? 3 : BlocksPerCol - _CurrentCol;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol-1].SetActive(false);
                }
                for (int i = 0; i< upperColLimit; i++) {
                    _Blocks[_CurrentRow-1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        else if (row == _CurrentRow + 1 && col == _CurrentCol - 1) { // Move Southwest, disable RIGHT ROWs & UP COLs
            if (_CurrentRow != 0 && _CurrentCol != BlocksPerCol - 1) { // If player used to be at Top or Right edge, do nothing
                int upperRowLimit = BlocksPerRow - _CurrentRow > 3 ? 3 : BlocksPerRow - _CurrentRow;
                int upperColLimit = BlocksPerCol - _CurrentCol > 3 ? 3 : BlocksPerCol - _CurrentCol;
                for (int i = 0; i < upperRowLimit; i++) {
                    _Blocks[i + _CurrentRow - 1, _CurrentCol+1].SetActive(false);
                }
                for (int i = 0; i< upperColLimit; i++) {
                    _Blocks[_CurrentRow-1, i + _CurrentCol - 1].SetActive(false);
                }
            }
        }
        
        _CurrentRow = row;
        _CurrentCol = col;
        StartCoroutine(EnableBlocks());
        // Wait for the specified interval
        yield return null;
    }

    
    
}
