package demo.cleanslice.datalayer.entities;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Ingredient")
public class Ingredient extends BaseModel {
    @DatabaseField(generatedId = true)
    private Long id;

    @DatabaseField(canBeNull = true)
    private String name;

    @DatabaseField(foreign = true, foreignAutoRefresh = true)
    public Recipe recipe;

    public Ingredient(String name) {
        this.name = name;
    }
}
