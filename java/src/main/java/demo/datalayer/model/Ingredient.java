package demo.datalayer.model;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Ingredient")
public class Ingredient extends BaseModel {
    @DatabaseField(generatedId = true)
    private int id;

    @DatabaseField(canBeNull = true)
    private String name;

    @DatabaseField(foreign = true, foreignAutoRefresh = true)
    private Recipe recipe;

    public Ingredient() {

    }
}
