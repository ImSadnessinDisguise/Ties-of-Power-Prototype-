using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities 
{
    //summary
    //Empty string debug check
    //</summary>

    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "is empty and must contain a value in project" + thisObject.name.ToString());
            return true;
        }
        return false; 
    }
    /// <summary>
    /// null value debug check
    /// </summary>

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectTocheck)
    {
        if (objectTocheck == null)
        {
            Debug.Log(fieldName + "is null and must contain a value in object" + thisObject.name.ToString());
            return true;

        }
        return false;
    }
    //list empty or contains null value check - return true if error

    public static bool ValidateCheckEnumerableValues (Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "is null in object" + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object" + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object" + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    ///<summary>
    /// positive value debug check - if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    /// </summary>
    
    public static bool ValidateCheckPositiveValue (Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + "must contain positive value or zero in object" + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + "must contain positive value in object" + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }


}
 