using System;
using System.Collections.Generic;
using System.Text;

namespace YangMvc
{
    public enum MongoDBs
    {
        GpsDB = 1,
        GpsRawDb = 2
    }

    public enum GpsDbCollections
    {
        GpsPoints = 1,
        GpsStates = 2,
        DeviceStatus = 3,

        /// <summary>
        /// 存储坐标矩阵信息，主键根据未纠偏的百度坐标生成。
        /// </summary>
        GpsCoords = 4,

        GpsPointDays = 5,
        GpsStateDays = 6,
        /// <summary>
        /// 存储关键设备状态（如报警产生时刻的状态）
        /// </summary>
        AlarmState = 7,

        TaskInfo = 8,

        GpsRawData = 10,

        /// <summary>
        /// 近60天内的点
        /// </summary>
        GpsPointPool = 11,

        UnRegisterDevices = 16
    }


}
