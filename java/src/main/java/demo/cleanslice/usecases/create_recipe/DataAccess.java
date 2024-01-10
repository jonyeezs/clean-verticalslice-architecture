package demo.cleanslice.usecases.create_recipe;

import java.sql.SQLException;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Service;

import demo.cleanslice.datalayer.RecipeDao;

@Service
@Qualifier("CreateRecipe")
public class DataAccess implements demo.cleanslice.common.DataAccess<RecipeBookDomain, List<UUID>>

{
    private RecipeDao recipeDao;

    public DataAccess(RecipeDao recipeDao) {
        this.recipeDao = recipeDao;
    }

    public RecipeBookDomain Retrieve() {
        return new RecipeBookDomain(this::findRecipeByTitle);
    }

    protected List<Recipe> findRecipeByTitle(String title) {
        try {
            return this.recipeDao.queryForEq("title", title)
                    .stream()
                    .map(r -> new Recipe(r.title, null))
                    .toList();
        } catch (SQLException e) {
            return Collections.emptyList();
        }
    }

    public List<UUID> Add(RecipeBookDomain domain) throws SQLException {
        List<demo.cleanslice.datalayer.entities.Recipe> recipes = domain.recipes
                .stream()
                .map(r -> {
                    var recipe = new demo.cleanslice.datalayer.entities.Recipe();
                    recipe.title = r.getTitle();
                    recipe.ingredients = Arrays.asList(r.getIngredients()).stream()
                            .map(i -> new demo.cleanslice.datalayer.entities.Ingredient(i.getName())).toList();
                    return recipe;
                }).toList();

        this.recipeDao.create(recipes);

        return recipes.stream().map(r -> r.id).toList();
    }
}
