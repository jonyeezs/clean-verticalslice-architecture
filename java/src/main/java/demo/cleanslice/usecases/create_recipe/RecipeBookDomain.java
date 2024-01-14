package demo.cleanslice.usecases.create_recipe;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

import javax.validation.constraints.NotBlank;

import lombok.Data;

public class RecipeBookDomain {
    private Function<String, List<Recipe>> getRecipesByTitle;

    public List<Recipe> recipes;

    public RecipeBookDomain(Function<String, List<Recipe>> getRecipesByTitle) {
        this.getRecipesByTitle = getRecipesByTitle;
        this.recipes = new ArrayList<Recipe>();
    }

    public void addRecipe(Recipe recipe) throws RecipeExistsException {
        var recipes = getRecipesByTitle.apply(recipe.getTitle());
        if (!recipes.isEmpty()) {
            throw new RecipeExistsException();
        }
        this.recipes.add(recipe);
    }
}

@Data
class Recipe {

    @NotBlank(message = "Title of the recipe is required")
    private final String title;
    private final Ingredient[] ingredients;
}

@Data
class Ingredient {
    private final String name;
    private final Integer amount;
    private final String unit;
}

class RecipeExistsException extends Exception {
    public RecipeExistsException() {
        super("Recipe with this title already exists. Try updating instead.");
    }
}
