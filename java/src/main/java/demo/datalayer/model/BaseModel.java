package demo.datalayer.model;

import java.util.Date;

import com.j256.ormlite.field.DataType;
import com.j256.ormlite.field.DatabaseField;

public class BaseModel {
    @DatabaseField(dataType = DataType.DATE)
    protected Date CreatedDate;

    @DatabaseField(dataType = DataType.DATE)
    protected Date ModifiedDate;
}
