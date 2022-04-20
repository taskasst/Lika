using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExNinja {

    /// <summary>
    /// Enum for relative locations where the vector of a VFCell can be drawn at
    /// </summary>
    public enum VectorDrawOrigin
    {
        BottomLeft,
        Center
    }

    /// <summary>
    /// A Vector2 field developed for Lika by Jeremy Gibson Bond
    /// Edited by the Lika programming team (Jacob Cousineau, Stefani Taskas, Jessica Clappison)
    /// </summary>
    public class XnVectorField2D : MonoBehaviour {
        public static ExNinja.XnVectorField2D instance = null;
        [Header("Inscribed")]
        public int      width;
        public int      height;
        public float    cellSize;
        public int boundaryVectorMagnitude;
        public int defaultNumRings;
        public bool     drawRays = false;
        public VectorDrawOrigin rayOriginType;
        public Vector2 downwardMotion = Vector2.zero;


        //[Header("Dynamic")]


        public List<VFCell[]> cells; // This is constructed as a List of VFCell columns 
        // so that the columns can be cycled to the front or back of the List as
        // the camera moves left and right.

        private int horizontalOffset; // running total of number of times we've swapped a piece from the left to the right
        // so, positive value means we've gone to the right and negative value means we've gone to the left

        private void Awake()
        {
            // Since there will be only one vector field, and many things
            // need to know it exists, we can use an instance of it
            if(instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Use this for initialization
        void Start () {
            horizontalOffset = 0;

            cells = new List<VFCell[]>();
            for (int i=0; i<width; i++) {
                cells.Add(new VFCell[height]);
                for (int j=0; j<height;j++)
                {
                    cells[i][j] = new VFCell();
                    cells[i][j].Init();
                }
            }

            InitCells();
    	}   	
    	
    	void FixedUpdate () {
            // iterate through columns
            for (int i=0; i<width; i++) {
                // check this column to see if it's on screen
                float colViewPortX = Camera.main.WorldToViewportPoint(GetLoc(i, 0)).x;
                if (colViewPortX < -0.2f)
                {
                    // this column is off screen to left -- remove it from the left side
                    VFCell[] temp = cells[i];
                    cells.RemoveAt(i);

                    // reset current vector of all cells in the shifted row
                    for (int row = 0; row < temp.Length; row++)
                    {
                        temp[row].ResetCell();
                    }

                    // re-attach it on the right side
                    cells.Add(temp);
                    horizontalOffset++;
                }
                else if (colViewPortX > 1.2f)
                {
                    // this column is off screen to right -- remove it from right side
                    VFCell[] temp = cells[i];
                    cells.RemoveAt(i);

                    // reset current vector of all cells in the shifted row
                    for (int row = 0; row < temp.Length; row++)
                    {
                        temp[row].ResetCell();
                    }

                    // re-attach it on the left side
                    cells.Insert(0, temp);
                    horizontalOffset--;
                }
                else
                {
                    for (int j = 0; j < height; j++)
                    {
                        cells[i][j].Dampen();
                    }
                }
            }
    	}

        private void Update()
        {
            if (drawRays) DebugDraw();
        }

        /// <summary>
        /// Initialize cells to have correct base vector values
        /// </summary>
        private void InitCells()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // bottom cell of each column should point directly up with lots of magnitude
                    if (j == 0)
                    {
                        cells[i][j].SetBase(Vector2.up * boundaryVectorMagnitude);
                    }
                    // top cell of each column should point directly down with lots of magnitude
                    else if (j + 1 == height)
                    {
                        //cells[i][j].vBase = Vector2.down * boundaryVectorMagnitude;
                        cells[i][j].SetBase(downwardMotion);
                    }
                    // all other cells should be at zero for now (may do some some of random slight x to create a windier effect later)
                    else
                    {
                        cells[i][j].SetBase(downwardMotion);
                    }

                    // regardless of what base we set, set the current value to that base
                    cells[i][j].ToBase();
                }
            }
        }


        /*void RandomizeCells(float magnitude=1f) {
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    cells[i][j].vBase = Random.insideUnitCircle * magnitude;
                    print(i+" "+j+" "+cells[i][j].vBase);
                }
            }
        }

        void SinCells() {
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    cells[i][j].vBase = new Vector2(
                        Mathf.Sin( ((float)i)/width * Mathf.PI * 1 ),
                        Mathf.Cos( ((float)j)/height * Mathf.PI * 1 )
                    );
                }
            }
        }*/

        /// <summary>
        /// Gets the location of a cell. NOTE that rotation and scale are NOT considered.
        /// </summary>
        public Vector3 GetLoc(int i, int j) {
            Vector3 pos = Vector3.zero;
            pos.x = ((float) i) - width*0.5f;
            pos.y = ((float) j) - height*0.5f;
            pos *= cellSize;
            pos += transform.position;
            pos.x += (cellSize * horizontalOffset);
            return pos;
        }

        /// <summary>
        /// Gets the cell based off of screen position.
        /// </summary>
        /// <param name="pos"> The cell position (in world coordinates) used to get cell array x,y </param>
        /// <returns>The x,y position in the array of cells</returns>
        public Vector2Int GetCell(Vector3 pos)
        {
            int cellX, cellY;
            pos.x -= (cellSize * horizontalOffset);
            pos -= transform.position;
            pos /= cellSize;
            cellX = (int)(pos.x + (width * 0.5f));
            cellY = (int)(pos.y + (height * 0.5f));
            Vector2Int testCell = new Vector2Int(cellX, cellY);
            return testCell;
        }

        public bool CellExists(Vector2Int cell)
        {
            return cell.x > 0 && cell.x < width && cell.y > 0 && cell.y < height;
        }

        /// <summary>
        /// Get the vector value of the cell at the given screen position
        /// </summary>
        /// <param name="pos">The world position we are trying to get the vector value of</param>
        /// <returns></returns>
        private Vector2 GetCellValue(Vector2Int cell)
        {
            Vector2 value = Vector2.zero;
            if (cell.x > 0 && cell.x < cells.Count)
            {
                if (cell.y > 0 && cell.y < cells[cell.x].Length)
                {
                    value = cells[cell.x][cell.y].vCurr;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the average of the vector values of the cell at a given position and the numRings cells in each direction out from that position
        /// </summary>
        /// <param name="pos"> The center position (in world coordinates) from which we will compute our average</param>
        /// <returns>The average of the vectors in the nine positions as described above</returns>
        public Vector2 GetAverageForceAt(Vector3 pos, int numRings = -1)
        {
            if (numRings == -1)
            {
                numRings = defaultNumRings;
            }

            Vector2 forceTotal = Vector2.zero;

            Vector2Int centerCell = GetCell(pos);
            for (int x = centerCell.x - numRings; x <= centerCell.x + numRings; x++)
            {
                for (int y = centerCell.y - numRings; y <= centerCell.y + numRings; y++)
                {
                    forceTotal += GetCellValue(new Vector2Int(x, y));
                }
            }

            return new Vector2(forceTotal.x / 9, forceTotal.y / 9);
        }

        void DebugDraw(int num=0) {
            DebugDraw(Color.green);
        }
        void DebugDraw(Color color) {
            Vector2 offUp = Vector2.up * 0.05f;
            Vector2 offRight = Vector2.right * 0.05f;
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {

                    // we can draw the ray from several places on the cell
                    // slightly different math for each
                    Vector3 rayOriginLoc;
                    switch(rayOriginType)
                    {
                        case VectorDrawOrigin.Center:
                            rayOriginLoc = GetLoc(i, j) + new Vector3(cellSize / 2f, cellSize / 2f, 0);
                            break;
                        case VectorDrawOrigin.BottomLeft:
                        default:
                            rayOriginLoc = GetLoc(i, j);
                            break;
                        
                    }
                    Debug.DrawRay(rayOriginLoc - (Vector3) offUp, offUp * 2, Color.gray);
                    Debug.DrawRay(rayOriginLoc - (Vector3) offRight, offRight * 2, Color.gray);
                    Debug.DrawRay(rayOriginLoc, (Vector3)cells[i][j].vCurr * cellSize, color);
                }
            }
        }
    }

    /**
     * Class that represents a single cell in the vector field
     */
    [System.Serializable]
    public class VFCell {
        static public float    dampenU = 0.02f;

        // base vector value that the cell will revert back to
        public Vector2 vBase { get; private set; }

        // current vector value -- current value of wind at this cell
        public Vector2 vCurr { get; private set; }

        // list of all stored base values (can be used to restore an old base value)
        private List<Vector2> storedBases;

        // flag for state in which the cell is locked to the base value and cannot be changed by user input
        private bool lockedToBase;

        // flag for state in which the wind at this cell is shifted to always face in a certain direction
        private bool reflect;

        // when reflect is true, the wind always points in this direction
        private Vector2 vReflect;

        // when reflect is true, the magnitude of the wind is multiplied by this value
        private float reflectMultiplier;

        /**
         * Initialization of VFCell
         */
        public void Init() {
            vBase = Vector2.zero;
            vCurr = Vector2.zero;
            storedBases = new List<Vector2>();
            lockedToBase = false;
            reflect = false;
            vReflect = Vector2.zero;
            reflectMultiplier = 0f;
        }

        /**
         * Reset a cell to a default state
         */
        public void ResetCell()
        {
            if (lockedToBase)
            {
                UnlockCell();
            }
            ToBase();
            storedBases = new List<Vector2>();
            reflect = false;
            vReflect = Vector2.zero;
            reflectMultiplier = 0;
        }

        /**
         * Dampen the cell's current value
         * 
         * Should be called in FixedUpdate
         */
        public void Dampen() {
            if (!lockedToBase && dampenU != 0)
            {
                vCurr = (1-dampenU)*vCurr + dampenU*vBase;
            }
        }

        /**
         * Set the base vector for the cell
         */
        public void SetBase(Vector2 baseForce)
        {
            vBase = baseForce;
        }

        /**
         * Make the current vector at the cell equal to the base vector
         */
        public void ToBase()
        {
            vCurr = vBase;
        }

        /**
         * Apply force to the vector field
         */
        public void ApplyForce(Vector2 force)
        {
            // only allow application of force if we are not in the lockedToBase state
            if (!lockedToBase)
            {
                // if reflect is true, we want the force to always go in the direction of vReflect
                // vReflect is multiplied by the incoming force's magnitude and reflectMultiplier to create our new force
                if (reflect)
                {
                    force = vReflect.normalized * force.magnitude * reflectMultiplier;
                }

                // move the current vector towards our passed in/calculated force vector
                vCurr = Vector2.MoveTowards(vCurr, force, force.magnitude);
            }
        }

        /**
         * Lock the current vector to the base vector value
         */
        public void LockToBase(Vector2 lockBase)
        {
            // store the current base so we can restore it later when this state is removed
            StoreCurrentBase();

            // set the base to the new base vector we passed in, then set current value to new base value
            SetBase(lockBase);
            ToBase();

            lockedToBase = true;
        }

        /**
         * Unlock the cell's current value so that it can vary from the base value
         */
        public void UnlockCell()
        {
            lockedToBase = false;

            // restore the previous base valuea and make the current value equal to that old base value
            RestoreBase();
            ToBase();
        }

        /**
         * Store the current base in the list of bases so we can restore the value later
         */
        public void StoreCurrentBase()
        {
            storedBases.Add(vBase);
        }

        /**
         * Restore the base vector value to the most recently stored vector in our list of stored bases
         */
        public void RestoreBase()
        {
            if (storedBases.Count > 0)
            {
                vBase = storedBases[storedBases.Count - 1];
                storedBases.RemoveAt(storedBases.Count - 1);
            }
        }

        /**
         * Turn reflection on
         * 
         * Reflection makes all force applied to this cell go in a specified direction -- only incoming magnitude matters
         */
        public void Reflect(Vector2 reflectVec, float multiplier)
        {
            reflect = true;

            // store the vector representing the direction the force will be reflected in and the multiplier placed on the magnitude
            vReflect = reflectVec;
            reflectMultiplier = multiplier;

            // if we are locked to the base, we have to do things slightly differently
            // store the current (locked) base, set the new base to be equal to the passed in vector with the old base's magnitude multiplied by a multiplier
            if (lockedToBase)
            {
                StoreCurrentBase();
                SetBase(vReflect.normalized * vBase.magnitude * multiplier);
                ToBase();
            }
        }

        /**
         * End reflection
         */
        public void StopReflect()
        {
            // restore the previous base
            RestoreBase();

            reflect = false;
            vReflect = Vector2.zero;
            reflectMultiplier = 0;
        }
    }
}