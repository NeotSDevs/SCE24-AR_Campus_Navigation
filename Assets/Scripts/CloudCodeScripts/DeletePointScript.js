
/*  DeletePointScript
 *  Parameters:
 *  Parameter name == "point_name" | Type == "STRING" |  required == "true"
 *  Parameter name == "data_id" | Type == "STRING" | required == "true"
 *  Description:
 *  The CloudCode script to delete a point (point_name) from the specified collection (data_id) used in CloudController.cs script
 */

/*
 * Include the lodash module for convenient functions such as "random"
 *  Check https://docs.unity.com/cloud-code/manual/scripts/reference/available-libraries
 *  to get a full list of libraries you can import in Cloud Code
 */
const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.4");

/*
 * CommonJS wrapper for the script. It receives a single argument, which can be destructured into:
 *  - params: Object containing the parameters provided to the script, accessible as object properties
 *  - context: Object containing the projectId, environmentId, environmentName, playerId and accessToken properties.
 *  - logger: Logging client for the script. Provides debug(), info(), warning() and error() log levels.
 */
module.exports = async ({ params, context, logger }) => {
  const { projectId, playerId, accessToken } = context;
  const cloudSaveApi = new DataApi(context);

  try {
    const customId = params.data_id;
    const requestedData = params.point_name;
    await cloudSaveApi.deleteCustomItem(requestedData,projectId, customId);

    return {
      responseMessage: "point " + requestedData + " deleted successfuly from " + customId
    };
  } catch (err) {
    logger.error("Error while calling out to Cloud Save", { "error.message": err.message });
    throw err;
  }
};