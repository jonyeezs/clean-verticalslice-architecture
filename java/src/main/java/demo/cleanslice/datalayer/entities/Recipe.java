package demo.cleanslice.datalayer.entities;

import java.util.Collection;
import java.util.UUID;

import com.j256.ormlite.field.DataType;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Recipe")
public class Recipe extends BaseModel {

    @DatabaseField(dataType = DataType.UUID_NATIVE, generatedId = true, readOnly = true)
    private UUID id;

    @DatabaseField(canBeNull = true)
    public String title;

    @ForeignCollectionField(eager = false)
    public Collection<Ingredient> ingredients;
}
