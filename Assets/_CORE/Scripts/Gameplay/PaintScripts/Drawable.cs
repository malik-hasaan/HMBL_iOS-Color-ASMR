using System.Linq;
using UnityEngine;
using System.Collections;

namespace FreeDraw
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Collider2D))]  
    
    // REQUIRES A COLLIDER2D to function
    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!

    public class Drawable : MonoBehaviour
    {
        public float CompletePercentage = 58;

        public Transform toolTip;

        public bool IsRaser;
        public GameObject RaserTool;

        public bool IsFaceCleanerSponge;
        public GameObject FaceCleanerSponge;

        public bool IsHairDryer;
        public GameObject HairDryerTool;

        public bool IsEyebrow;
        public GameObject EyebrowTool;

        public bool IsEyeshade;
        public GameObject EyeshadeTool;

        public bool IsEyelash;
        public GameObject EyelashTool;

        public bool isWithoutTool;

        public void SetToolTip(GameObject tip)
        {
            toolTip = tip.transform;
        }

        public void ResetProgress ()
        {
            count = 0;
            percentage = 0;
        }

        // PEN COLOUR
        public static Color Pen_Colour = Color.red;     // Change these to change the default drawing settings

        // PEN WIDTH (actually, it's a radius, in pixels)
        public static int Pen_Width = 14;


        public delegate void Brush_Function(Vector2 world_position);

        // This is the function called when a left click happens
        // Pass in your own custom one to change the brush type
        // Set the default function in the Awake method
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent

        // Used to reference THIS specific file without making all methods static
        //public static Drawable drawable;


        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;


        //////////////////////////////////////////////////////////////////////////////
        // BRUSH TYPES. Implement your own here


        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector2 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            cur_colors = drawable_texture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previous_drag_position == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();

            // 4. If dragging, update where we were previously
            previous_drag_position = pixel_pos;
        }


        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 world_point)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            cur_colors = drawable_texture.GetPixels32();
            //Debug.Log(cur_colors.Length);
            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }


        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            current_brush = PenBrush;
        }
        //////////////////////////////////////////////////////////////////////////////


        private void OnTriggerStay2D(Collider2D collision)
        {
            //if (collision.gameObject.name == "FaceCleanerSponge" || collision.gameObject.name == "EyebrowTool"
            //   || collision.gameObject.name == "EyeshadeTool" || collision.gameObject.name == "EyelashTool" || collision.gameObject.name == "Tool") 

            //if (collision.gameObject.name == "DragTool" || collision.gameObject.name == "Tool") 
            if (collision.gameObject.tag == "ScratchTool")// || collision.gameObject.name == "Tool") 
            {
                Debug.LogError("HAMMAD");
                ChangePenPattern(0);
                {
                    // Convert mouse coordinates to world coordinates
                    //Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 mouse_world_position = toolTip.transform.position;

                    // Check if the current mouse position overlaps our image
                    Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);

                    if (hit != null && hit.transform != null)
                    {
                        // We're over the texture we're drawing on!
                        // Use whatever function the current brush is
                        current_brush(mouse_world_position);
                    }

                    else
                    {
                        // We're not over our destination texture
                        previous_drag_position = Vector2.zero;
                        if (!mouse_was_previously_held_down)
                        {
                            // This is a new drag where the user is left clicking off the canvas
                            // Ensure no drawing happens until a new drag is started
                            no_drawing_on_current_drag = true;
                        }
                    }
                }

                percentage = (float)count / totalNumOfPixels * 100;

                if (percentage > CompletePercentage)
                {
                    SetReferenceImageComplete();
                }

            }

            else if (collision.gameObject.name == "RaserTool")
            {
                ChangePenPattern(1);
                Vector2 mouse_world_position = toolTip.transform.position;
                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    // Use whatever function the current brush is
                    current_brush(mouse_world_position);
                    //   AchievementUpdater.Instance.Add("Draw Lines", 10f);
                }
                else
                {
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
                percentage = (float)count / totalNumOfPixels * 100;
                if (percentage > CompletePercentage)
                {
                    SetReferenceImageComplete();
                }
            }

            if (collision.gameObject.name == "HairDryerTool")
            {
                ChangePenPattern(0);
                {
                    // Convert mouse coordinates to world coordinates
                    //Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 mouse_world_position = toolTip.transform.position;

                    // Check if the current mouse position overlaps our image
                    Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                    if (hit != null && hit.transform != null)
                    {
                        // We're over the texture we're drawing on!
                        // Use whatever function the current brush is
                        current_brush(mouse_world_position);
                    }
                    else
                    {
                        // We're not over our destination texture
                        previous_drag_position = Vector2.zero;
                        if (!mouse_was_previously_held_down)
                        {
                            // This is a new drag where the user is left clicking off the canvas
                            // Ensure no drawing happens until a new drag is started
                            no_drawing_on_current_drag = true;
                        }
                    }
                }
                percentage = (float)count / totalNumOfPixels * 100;
                if (percentage > CompletePercentage)
                {
                    SetReferenceImageComplete();
                }

            }

        }

        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (collision.gameObject.name == "Marker")
        //    {
        //        print("OUTER");
        //        ChangePenPattern(0);
        //        bool mouse_held_down = Input.GetMouseButton(0);
        //        if (mouse_held_down && !no_drawing_on_current_drag)
        //        //if (!no_drawing_on_current_drag)

        //        {
        //            print("Inner");
        //            // Convert mouse coordinates to world coordinates
        //            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //            // Check if the current mouse position overlaps our image
        //            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
        //            if (hit != null && hit.transform != null)
        //            {
        //                // We're over the texture we're drawing on!
        //                // Use whatever function the current brush is
        //                current_brush(mouse_world_position);

        //            }

        //            else
        //            {
        //                // We're not over our destination texture
        //                previous_drag_position = Vector2.zero;
        //                if (!mouse_was_previously_held_down)
        //                {
        //                    // This is a new drag where the user is left clicking off the canvas
        //                    // Ensure no drawing happens until a new drag is started
        //                    no_drawing_on_current_drag = true;
        //                }
        //            }
        //        }
        //        // Mouse is released
        //        else if (!mouse_held_down)
        //        {
        //            previous_drag_position = Vector2.zero;
        //            no_drawing_on_current_drag = false;
        //        }
        //        mouse_was_previously_held_down = mouse_held_down;
        //        percentage = (float)count / totalNumOfPixels * 100;
        //        if (percentage > 50)
        //        {
        //            SetReferenceImageComplete();
        //        }
        //    }
        //    if (collision.gameObject.name == "Duster")
        //    {
        //        ChangePenPattern(1);
        //        bool mouse_held_down = Input.GetMouseButton(0);
        //        if (mouse_held_down && !no_drawing_on_current_drag)
        //        {
        //            // Convert mouse coordinates to world coordinates
        //            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //            // Check if the current mouse position overlaps our image
        //            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
        //            if (hit != null && hit.transform != null)
        //            {
        //                // We're over the texture we're drawing on!
        //                // Use whatever function the current brush is
        //                current_brush(mouse_world_position);
        //             //   AchievementUpdater.Instance.Add("Draw Lines", 10f);
        //            }

        //            else
        //            {
        //                // We're not over our destination texture
        //                previous_drag_position = Vector2.zero;
        //                if (!mouse_was_previously_held_down)
        //                {
        //                    // This is a new drag where the user is left clicking off the canvas
        //                    // Ensure no drawing happens until a new drag is started
        //                    no_drawing_on_current_drag = true;
        //                }
        //            }
        //        }
        //        // Mouse is released
        //        else if (!mouse_held_down)
        //        {
        //            previous_drag_position = Vector2.zero;
        //            no_drawing_on_current_drag = false;
        //        }
        //        mouse_was_previously_held_down = mouse_held_down;
        //        percentage = (float)count / totalNumOfPixels * 100;
        //        if (percentage > 50)
        //        {
        //            SetReferenceImageComplete();
        //        }
        //    }

        //}
        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function

        void Update()
        {
            if(isWithoutTool)
            {
                // Is the user holding down the left mouse button?
                bool mouse_held_down = Input.GetMouseButton(0);
                if (mouse_held_down && !no_drawing_on_current_drag)
                {
                    // Convert mouse coordinates to world coordinates
                    Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Check if the current mouse position overlaps our image
                    Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                    if (hit != null && hit.transform != null)
                    {
                        // We're over the texture we're drawing on!
                        // Use whatever function the current brush is
                        current_brush(mouse_world_position);
                    }

                    else
                    {
                        // We're not over our destination texture
                        previous_drag_position = Vector2.zero;
                        if (!mouse_was_previously_held_down)
                        {
                            // This is a new drag where the user is left clicking off the canvas
                            // Ensure no drawing happens until a new drag is started
                            no_drawing_on_current_drag = true;
                        }
                    }
                }
                // Mouse is released
                else if (!mouse_held_down)
                {
                    previous_drag_position = Vector2.zero;
                    no_drawing_on_current_drag = false;
                }
                mouse_was_previously_held_down = mouse_held_down;
                percentage = (float)count / totalNumOfPixels * 100;
                if (percentage > CompletePercentage)
                {
                    SetReferenceImageComplete();
                }
            }
        }

        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }

        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos >= cur_colors.Length || array_pos < 0)
                return;

            //if (cur_colors[array_pos].a < 0.5f)
            //{
            //    return;
            //}
            //if (isColor)
            //{
            //    //print(cur_colors[array_pos]);
            //    if (!cur_colors[array_pos].Equals((Color32)color))
            //    {
            //        count++;
            //    }
            //    cur_colors[array_pos] = color;
            //}
            //else
            {
                if (referenceImageColorsArray.Length > 0)
                {
                    if (!cur_colors[array_pos].Equals(referenceImageColorsArray[array_pos]))
                    {
                        count++;
                    }

                    cur_colors[array_pos] = referenceImageColorsArray[array_pos];
                }

            }


            //cur_colors[array_pos] = referenceImageColorsArray[array_pos];
        }

        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }

        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)

        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }

        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }

        public void ChangePenPattern(int index)
        {
            referenceImageTexture = referenceImage[index].texture;
            referenceImageColorsArray = referenceImageTexture.GetPixels32();
            //   GetNumberOfPixelToChange();
        }

        public void GetNumberOfPixelToChange()
        {
            StartCoroutine(GetPixel());
        }

        IEnumerator GetPixel()
        {
            totalNumOfPixels = 0;
            count = 0;
            percentage = 0;
            drawableImageColorsArray = drawable_texture.GetPixels32();
            //Debug.Log("Total Num : "+totalNumOfPixels+" Total Pixel "+ drawableImageColorsArray.Length);
            if (isColor)
            {
                totalNumOfPixels = drawableImageColorsArray.Count();
            }
            else
            {
                for (int i = 0; i < drawableImageColorsArray.Length; i++)
                {
                    if (!referenceImageColorsArray[i].Equals(drawableImageColorsArray[i]))
                    {
                        if (referenceImageColorsArray[i].a != 0)
                            totalNumOfPixels++;
                    }
                }
            }


            yield return null;
        }

        // Changes every pixel to be the reset colour

        public void ResetCanvas()
        {
            drawable_texture.SetPixels32(defaultImageColorsArray);
            drawable_texture.Apply();
        }

        public void SetReferenceImageComplete()
        {
            if (IsRaser)
            {
                IsRaser = false;
                RaserTool.GetComponent<PaintToolDrag>().MoveBackRaser();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
            if (IsFaceCleanerSponge)
            {
                IsFaceCleanerSponge = false;
                FaceCleanerSponge.GetComponent<PaintToolDrag>().MoveBackFaceCleaner();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
            if (IsHairDryer)
            {
                IsHairDryer = false;
                HairDryerTool.GetComponent<PaintToolDrag>().MoveBackHairDryer();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
            if (IsEyebrow)
            {
                IsEyebrow = false;
                EyebrowTool.GetComponent<PaintToolDrag>().MoveBackEyebrowTool();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
            if (IsEyeshade)
            {
                IsEyeshade = false;
                EyeshadeTool.GetComponent<PaintToolDrag>().MoveBackEyeShadeTool();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
            if (IsEyelash)
            {
                IsEyelash = false;
                EyelashTool.GetComponent<PaintToolDrag>().MoveBackEyeLashesTool();
                drawable_texture.SetPixels32(referenceImageColorsArray);
                drawable_texture.Apply();
                ResetProgress();
            }
        }

        public static bool isColor = true;
        public Sprite defaultImage;
        public Sprite[] referenceImage;
        Texture2D referenceImageTexture;

        Color32[] referenceImageColorsArray;
        Color32[] defaultImageColorsArray;
        Color32[] drawableImageColorsArray;
        public int totalNumOfPixels;
        public int count;
        public float percentage;
        
        void Awake()
        {
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;
            count = 0;
            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;
            referenceImageTexture = referenceImage[0].texture;
            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            referenceImageColorsArray = referenceImageTexture.GetPixels32();
            defaultImageColorsArray = defaultImage.texture.GetPixels32();

            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play)
                ResetCanvas();

             GetNumberOfPixelToChange();

        }
    }
}